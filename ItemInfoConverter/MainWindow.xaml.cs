using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace ItemInfoConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool running;
        CommonOpenFileDialog dialog;
        public MainWindow()
        {
            EncodingProvider provider = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(provider);
            running = false;
            dialog = new CommonOpenFileDialog { IsFolderPicker = true, EnsurePathExists = true };
            InitializeComponent();
        }

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.BeginInvoke(async () =>
            {
                try
                {
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok && !running)
                    {
                        pbStatus.Value = 0;
                        running = true;
                        pbStatus.Visibility = Visibility.Visible;
                        var converter = new Converter(dialog.FileName);
                        txtStatus.Text = "idnum2itemdesctable.txt...";
                        await converter.ReadIdentifiedDescriptionFile();
                        pbStatus.Value += 12.5;
                        txtStatus.Text = "idnum2itemresnametable.txt...";
                        await converter.ReadIdentifiedResourceFile();
                        pbStatus.Value += 12.5;
                        txtStatus.Text = "idnum2itemdisplaynametable.txt...";
                        await converter.ReadIdentifiedDisplayFile();
                        pbStatus.Value += 12.5;
                        txtStatus.Text = "idnum2itemdesctable.txt...";
                        await converter.ReadUnidentifiedDescriptionFile();
                        pbStatus.Value += 12.5;
                        txtStatus.Text = "num2itemresnametable.txt...";
                        await converter.ReadUnidentifiedResourceFile();
                        pbStatus.Value += 12.5;
                        txtStatus.Text = "num2itemdisplaynametable.txt...";
                        await converter.ReadUnidentifiedDisplayFile();
                        pbStatus.Value += 12.5;
                        txtStatus.Text = "itemslotcounttable.txt...";
                        await converter.ReadSlotCountFile();
                        pbStatus.Value += 12.5;
                        txtStatus.Text = "item_db.conf...";
                        await converter.ReadItemDbConf();
                        pbStatus.Value += 10;
                        txtStatus.Text = "Gerando iteminfo.lua...";
                        await converter.GenerateItemInfo();
                        pbStatus.Value += 2.5;
                        txtStatus.Text = "Finalizado!";
                        MessageBox.Show("O iteminfo.lua foi gerado no diretório escolhido.", "Sucesso!", MessageBoxButton.OK);
                        running = false;
                    }
                }
                catch (Exception ex)
                {
                    await LogError(ex);
                    MessageBox.Show("Ocorreu um erro. Um log foi criado na raiz do programa.", "ERRO", MessageBoxButton.OK);
                    running = false;
                }
            }, DispatcherPriority.ContextIdle);
        }

        private async Task LogError(Exception ex)
        {
            string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            message += string.Format("Message: {0}", ex.Message);
            message += Environment.NewLine;
            message += string.Format("StackTrace: {0}", ex.StackTrace);
            message += Environment.NewLine;
            message += string.Format("Source: {0}", ex.Source);
            message += Environment.NewLine;
            message += string.Format("TargetSite: {0}", ex.TargetSite.ToString());
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            using (StreamWriter writer = new StreamWriter("error.log", true))
            {
                await writer.WriteAsync(message);
                writer.Close();
            }
        }
    }
}
