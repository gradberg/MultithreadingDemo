using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MultithreadingDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // ---- Set this GUI window's thread priority as higher

            // ---- Startup timer event to periodically update the GUI by querying the state
            //      of the processing strategies. (The alternative is to have the processor
            //      and generator PROVIDE the information on their own schedule, but that is
            //      undesirable as different processing strategies could have significantly
            //      different update rates based on their performance. So having a polling-method
            //      of asking them for their state is ideal)
            //       
            
            // ---- There would be a max time each strategy would run, and then automatically switch to the next one.
            // The user can delay that happening, or manually tell it to switch. This would use just a DateTime variable
            // to store when it should next change, and then the timer-event to update the GUI would check this value to
            // know if it was time to shut down and change. Perhaps shutting-down should be an async-await thing?
        }


        // Thread strategy picked - shut down old and start new
        // If Old thread strategy
        //   Signal to the generator to shut down (it wraps up any in-memory structures)
        //   Signal to the process to shut down

        // Start up newly selected processor
        // Start up new generator


    }
}
