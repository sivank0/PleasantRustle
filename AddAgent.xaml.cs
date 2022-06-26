using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PleasantRustle
{
    /// <summary>
    /// Логика взаимодействия для AddAgent.xaml
    /// </summary>
    public partial class AddAgent : Window
    {
        public string Path { get; private set; }
        public int ID { get; private set; }
        public void AddTypes()
        {
            agentType.Items.Add("Все типы");
            agentType.SelectedIndex = 0;
            using (PleasantRustleEntities db = new PleasantRustleEntities())
            {
                List<AgentTypes> agentTypes = db.AgentTypes.ToList();
                foreach (AgentTypes type in agentTypes)
                {
                    agentType.Items.Add(type.TypeName);
                }
            }
        }
        public AddAgent()
        {
            InitializeComponent();
            remove.Visibility = Visibility.Collapsed;
            changeAgent.Visibility = Visibility.Collapsed;
            AddTypes();
        }
        public AddAgent(Agents agent)
        {
            InitializeComponent();
            AddTypes();
            ID = agent.Id;
            agentName.Text = agent.NameCompany;
            agentType.SelectedIndex = (int)agent.AgentTypeId;
            priority.Text = agent.Priority.ToString();
            adress.Text = agent.Adress;
            INN.Text = agent.INN;
            KPP.Text = agent.KPP;
            directorName.Text = agent.Director;
            phone.Text = agent.Phone;
            email.Text = agent.Email;
            addAgent.Visibility = Visibility.Collapsed;
        }
        private void choosePath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg)|*.png;*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
                Path = openFileDialog.FileName;
            logo.Source = new BitmapImage(new Uri(Path));
        }
        private void changeAgent_Click(object sender, RoutedEventArgs e)
        {
            string iFile = Path;
            byte[] imageData = null;
            FileInfo fInfo = new FileInfo(iFile);
            long numBytes = fInfo.Length;
            FileStream fStream = new FileStream(iFile, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fStream);
            imageData = br.ReadBytes((int)numBytes);

            using (PleasantRustleEntities db = new PleasantRustleEntities())
            {
                Agents agent = db.Agents.FirstOrDefault(a => a.Id == ID);
                agent.NameCompany = agentName.Text;
                agent.AgentTypeId = agentType.SelectedIndex;
                agent.Adress = adress.Text;
                agent.INN = INN.Text;
                agent.KPP = KPP.Text;
                agent.Director = directorName.Text;
                agent.Phone = phone.Text;
                agent.Email = email.Text;
                agent.Logo = imageData;
                agent.Priority = Convert.ToInt32(priority.Text);
                db.SaveChanges();
                MessageBox.Show($"Пользователь: {agent.NameCompany} изменен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            Close();
        }

        private void addAgent_Click(object sender, RoutedEventArgs e)
        {
            string iFile = Path;
            byte[] imageData = null;
            FileInfo fInfo = new FileInfo(iFile);
            long numBytes = fInfo.Length;
            FileStream fStream = new FileStream(iFile, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fStream);
            imageData = br.ReadBytes((int)numBytes);

            using (PleasantRustleEntities db = new PleasantRustleEntities())
            {
                Agents agent = new Agents
                {
                    Id = db.Agents.Max(a => a.Id) + 1,
                    NameCompany = agentName.Text,
                    AgentTypeId = agentType.SelectedIndex,
                    Adress = adress.Text,
                    INN = INN.Text,
                    KPP = KPP.Text,
                    Director = directorName.Text,
                    Phone = phone.Text,
                    Email = email.Text,
                    Logo = imageData,
                    Priority = Convert.ToInt32(priority.Text),
                    Discount = 0
                };
                db.Agents.Add(agent);
                db.SaveChanges();
                MessageBox.Show($"Пользователь: {agent.NameCompany} создан", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Owner.Show();
        }

        private void remove_Click(object sender, RoutedEventArgs e)
        {
            using(PleasantRustleEntities db = new PleasantRustleEntities())
            {
                var productRealiz = db.ProductRealization.ToList();
                if (productRealiz.Where(p => p.AgentId == ID).Count() == 0)
                    MessageBox.Show($"Удаление {agentName.Text} невозможно", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    Agents selectedUser = new Agents();
                    selectedUser = db.Agents.FirstOrDefault(a => a.Id == ID);
                    db.Agents.Remove(selectedUser);
                    db.SaveChanges();
                }
            }
        }
    }
}
