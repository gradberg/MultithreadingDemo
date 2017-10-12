using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultithreadingDemo.EventGenerator
{
    /// <summary>
    /// Object that stores all information of an order that is still open, such that events can be produced
    /// for it.
    /// </summary>
    internal class OpenOrder
    {
        public enum SideEnum
        {
            Buy,
            Sell,
            //SellShort // not really concerned with this, as position would be needed as well.
        }

        internal readonly string _traderTag;
        internal readonly string _productname;
        internal readonly SideEnum _side;
        internal readonly UInt64 _orderNumber;

        internal decimal _price;
        internal int _totalQuantity;
        internal int _remainingQuantity; // LeavesQty
        internal int _filledQuantity = 0; // CumQty
        
        public OpenOrder(string traderTag, string productName, decimal price, int quantity, SideEnum side, UInt64 orderNumer)
        {
            this._orderNumber = orderNumer;
            this._traderTag = traderTag;
            this._productname = productName;
            this._totalQuantity = quantity;
            this._remainingQuantity = quantity;
            this._side = side;
            this._price = price;
        }

        public readonly string FIX_DELIMITER = ((char)01).ToString();
        private const string TRADING_FIRM_NAME = "FAKE_PROP_TRADING_FIRM";
        private const string EXCHANGE_NAME = "FAKE_EXCHANGE";
        

        public string[] NewEvent()
        {
            return new string[] {
                AddFixHeaderFooter(String.Join(FIX_DELIMITER,
                    "35=D", // New Order
                    "38=" + this._totalQuantity.ToString(), // OrderQty
                    "44="+this._price.ToString(), // Price
                    GetCommonSentFields()
                )),
                AddFixHeaderFooter(String.Join(FIX_DELIMITER,
                    "35=8", // Execution Report
                    "150=0", // NEW ack
                    GetCommonReceivedFields()
                ))
            };
        }

        public string[] ModifyEvent(Decimal newPrice, int quantityDiff)
        {
            this._totalQuantity = this._totalQuantity + quantityDiff;
            this._remainingQuantity += quantityDiff;

            if (this._remainingQuantity <= 0)
            {
                // To simply the coding structure, I won't allow modifies to bring the remaining quantity below zero. Instead,
                // the generator code should simply cancel the order.
                throw new InvalidOperationException("ModifyEvent should not be called to implicitly cancel the order with qty <= 0");
            }

            return new string[] {
                AddFixHeaderFooter(String.Join(FIX_DELIMITER,
                    "35=G", // Modify Order
                    "38="+this._remainingQuantity.ToString(), // OrderQty
                    "44=" + this._price.ToString(), // Price
                    GetCommonSentFields()
                )),
                AddFixHeaderFooter(String.Join(FIX_DELIMITER,
                    "35=8", // Execution Report
                    "150=5", // (Replace) MODIFY ack
                    GetCommonReceivedFields()
                ))
            };
        }

        public string[] PartialFillEvent(int quantityFilled)
        {
            this._remainingQuantity -= quantityFilled;
            this._filledQuantity += quantityFilled;

            if (this._remainingQuantity <= 0)
            {
                // To simplify the coding structure, I won't allow partial fills to bring the remaining quantity below zero. As that
                // means the calling code should intentionally generate a complete fill.
                throw new InvalidOperationException("PartialFillEvent should not be called to create a complete fill with qty <= 0");
            }

            // Don't bother with setting remaining quantity
            return new string[] {
                AddFixHeaderFooter(String.Join(FIX_DELIMITER,
                    "14=" + this._filledQuantity.ToString(), // CumQty
                    "35=8", // Execution Report
                    "39=3", // Partially Filled
                    "150=F", // Traded/Filled
                    "151=" + this._remainingQuantity.ToString(), // LeavesQty
                    GetCommonReceivedFields()
                ))
            };
        }

        public string[] CompleteFillEvent()
        {
            this._filledQuantity += this._remainingQuantity;
            this._remainingQuantity = 0;

            // Don't bother with setting remaining quantity
            return new string[] {
                AddFixHeaderFooter(String.Join(FIX_DELIMITER,
                    "14=" + this._filledQuantity.ToString(), // CumQty
                    "35=8", // Execution Report
                    "39=7", // Fully Filled
                    "150=F", // Traded/Filled
                    "151=" + this._remainingQuantity.ToString(), // LeavesQty
                    GetCommonReceivedFields()
                ))
            };
        }

        public string[] CancelEvent()
        {
            return new string[] {
                AddFixHeaderFooter(String.Join(FIX_DELIMITER,
                    "35=F", // Cancel Request Order
                    GetCommonSentFields()
                )),
                AddFixHeaderFooter(String.Join(FIX_DELIMITER,
                    "35=8", // Cancel Request Order
                    "39=4", // Cancelled
                    "150=4", // CANCEL ACK
                    GetCommonReceivedFields()
                )),
            };
        }

        private string AddFixHeaderFooter(string messageBody)
        {
            var wrappedMessageBody = FIX_DELIMITER + messageBody + FIX_DELIMITER;
            int bodyLength = wrappedMessageBody.Length;

            var prechecksumMessage =
                "8=FIX.4.4" + FIX_DELIMITER +
                "9=" + bodyLength.ToString() +
                wrappedMessageBody;

            int checksum = prechecksumMessage.Sum(x => (int)x) % 256;
            return prechecksumMessage + "10=" + checksum.ToString("000") + FIX_DELIMITER;
        }

        private string GetCommonSentFields()
        {
            return String.Join(FIX_DELIMITER,
                "11=" + this._orderNumber.ToString(),
                "49=" + TRADING_FIRM_NAME,
                "50=" + this._traderTag,
                "53=" + this._remainingQuantity,
                "54=" + this._side,
                "55=" + this._productname,
                "56=" + EXCHANGE_NAME
            );
        }

        private string FormattedSide
        {
            get
            {
                switch (this._side) {
                    case SideEnum.Buy:
                        return 1;
                    case SideEnum.Sell:
                        return 2;
                    default:
                        throw new InvalidOperationException("Unknown Side: " + this._side.ToString());
                }
            }
        }

        private string GetCommonReceivedFields()
        {
            return String.Join(FIX_DELIMITER,
                "11=" + this._orderNumber.ToString(),
                "49=" + EXCHANGE_NAME,
                "55=" + this._productname,
                "56=" + TRADING_FIRM_NAME,
                "57=" + this._traderTag
            );
        }
    }
}
