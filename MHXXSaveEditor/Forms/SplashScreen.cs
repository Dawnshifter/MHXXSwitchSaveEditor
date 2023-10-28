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
        static public void SaveType(int GameSystem)
        {
            switch (GameSystem)
            {
                case 1:
                    splashForm.label1.Text = "Loading your save file...\nPlease wait...\n\nSave Type: MHXX 3DS";
                    break;
                case 2:
                    splashForm.label1.Text = "Loading your save file...\nPlease wait...\n\nSave Type: MHXX Switch";
                    break;
                case 3:
                    splashForm.label1.Text = "Loading your save file...\nPlease wait...\n\nSave Type: MHGU Switch";
                    break;
                case 4:
                    splashForm.Text = "Error";
                    splashForm.label1.Text = "Invalid save format";
                    break;
                default:
                    splashForm.label1.Text = "Loading your save file...\nPlease wait...\n\nSave Type: [Loading]";
                    break;
            }
        }
    }
}
