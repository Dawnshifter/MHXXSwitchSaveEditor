using System.Windows.Forms;
using System.Threading;

namespace MHXXSaveEditor.Forms
{
    public partial class SplashScreen : Form
    {
        private delegate void CloseDelegate();

        private static SplashScreen splashForm;

        static public void ShowSplashScreen()
        {
            if (splashForm != null)
                return;
            Thread thread = new Thread(new ThreadStart(ShowForm))
            {
                IsBackground = true
            };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        static private void ShowForm()
        {
            splashForm = new SplashScreen();
            Application.Run(splashForm);
        }
        static public void CloseForm()
        {
            splashForm.Invoke(new CloseDelegate(CloseFormInternal));
        }

        static private void CloseFormInternal()
        {
            splashForm.Close();
            splashForm = null;
        }
        public SplashScreen()
        {
            InitializeComponent();
        }
    }
}
