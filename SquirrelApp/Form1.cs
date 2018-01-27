using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Squirrel;

namespace SquirrelApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Load += Form1_Load;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            var assembly = Assembly.GetExecutingAssembly();
            lblVersion.Text = assembly.GetName().Version.ToString(3);

            var settings = Settings.Current;

            txtMaster.Text = settings.Version;
            await UpdateApp();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var settings = Settings.Current;

            settings.Version = txtMaster.Text;
            settings.Years = DateTime.Now.Second;

            settings.Save();
        }

        private async Task UpdateApp()
        {
            var updateManager = new UpdateManager(@"\\Sistema4-pc\compartida desarrollo\Actualizar\app");
            using (updateManager)
            {
                var updateInfo = await updateManager.CheckForUpdate();
                if (updateInfo == null || !updateInfo.ReleasesToApply.Any())
                {
                    return;
                }

                MessageBox.Show("Actualizacion Encontrada");

#if DEBUG
                return;
#endif
                var releases = updateInfo.ReleasesToApply;
                await updateManager.DownloadReleases(releases, _ => { });
                await updateManager.ApplyReleases(updateInfo, _ => { });

                MessageBox.Show("Se reiniciara la aplicacion");
                UpdateManager.RestartApp();
            }
        }
    }
}
