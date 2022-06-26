using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace PleasantRustle
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Agents> items = new ObservableCollection<Agents>();
        public ObservableCollection<Agents> Items
        {
            get { return new ObservableCollection<Agents>(items); }
        }

        private void AddAgentsToItems()
        {   
            using (PleasantRustleEntities db = new PleasantRustleEntities())
            {
                var agents = db.Agents.ToList();
                foreach (var agent in agents)
                {
                    DiscountCalc(agent);
                    items.Add(agent);
                }
            }
        }
        private void DiscountCalc(Agents agent)
        {
            using (PleasantRustleEntities db = new PleasantRustleEntities())
            {
                Double sales = 0;
                var productRealiz = db.ProductRealization.Where(pr => pr.AgentId == agent.Id).ToList();
                foreach (var item in productRealiz)
                {
                    var products = db.Products.FirstOrDefault(p => p.Id == item.ProductId);
                    sales += Convert.ToDouble(products.PriceMin) * Convert.ToDouble(item.Count);
                }
                if (sales <= 10000)
                    agent.Discount = 0;
                else if (sales > 10000 && sales < 50000)
                    agent.Discount = 5;
                else if (sales >= 50000 && sales < 150000)
                    agent.Discount = 10;
                else if (sales >= 150000 && sales < 500000)
                    agent.Discount = 20;
                else if (sales >= 500000)
                    agent.Discount = 25;
            }
        }
        private void SortingItemsAdd()
        {
            sort.Items.Add("По умолчанию");
            sort.SelectedIndex = 0;
            sort.Items.Add("Наименование по возрастанию");
            sort.Items.Add("Наименование по убыванию");
            sort.Items.Add("Размер скидки по возрастанию");
            sort.Items.Add("Размер скидки по убыванию");
            sort.Items.Add("Приоритет по возрастанию");
            sort.Items.Add("Приоритет по убыванию");
        }
        private void FilterItemsAdd()
        {
            filter.Items.Add("Все типы");
            filter.SelectedIndex = 0;
            using (PleasantRustleEntities db = new PleasantRustleEntities())
            {
                List<AgentTypes> agentTypes = db.AgentTypes.ToList();
                foreach (AgentTypes type in agentTypes)
                {
                    filter.Items.Add(type.TypeName);
                }
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            AddAgentsToItems();
            SortingItemsAdd();
            FilterItemsAdd();
            agentsList.ItemsSource = Items.Take(5);
        }

        int page = 1;
        private void nextPage_Click(object sender, RoutedEventArgs e)
        {
            agentsList.ItemsSource = Items.Skip(5*page).Take(5);
            page++;
        }

        private void previousPage_Click(object sender, RoutedEventArgs e)
        {
            page--;
            agentsList.ItemsSource = Items.Skip(5*page).Take(5);
        }

        private void search_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (search.Text == String.Empty)
                agentsList.ItemsSource = Items.Skip(5 * page).Take(5);
            else
                agentsList.ItemsSource = Items.Where(i => i.NameCompany.Contains(search.Text) || i.Phone.Contains(search.Text)).Take(5);   
        }

        private void sort_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            page = 1;
            switch (sort.SelectedIndex)
            {
                case 0:
                    agentsList.ItemsSource = Items.Skip(5*page).Take(5);
                    break;
                case 1:
                    agentsList.ItemsSource = Items.OrderBy(i => i.NameCompany).Skip(5*page).Take(5);
                    break;
                case 2:
                    agentsList.ItemsSource = Items.OrderByDescending(i => i.NameCompany).Skip(5 * page).Take(5);
                    break;
                case 3:
                    agentsList.ItemsSource = Items.OrderBy(i => i.Discount).Skip(5 * page).Take(5);
                    break;
                case 4:
                    agentsList.ItemsSource = Items.OrderByDescending(i => i.Discount).Skip(5 * page).Take(5);
                    break;
                case 5:
                    agentsList.ItemsSource = Items.OrderBy(i => i.Priority).Skip(5 * page).Take(5);
                    break;
                case 6:
                    agentsList.ItemsSource = Items.OrderByDescending(i => i.Priority).Skip(5 * page).Take(5);
                    break;
            }
        }

        private void filter_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            using(PleasantRustleEntities db = new PleasantRustleEntities())
            {
                var types = db.AgentTypes.ToList();
                switch (filter.SelectedIndex)
                {
                    case 0:
                        agentsList.ItemsSource = Items.Take(5);
                        break;
                    case 1:
                        agentsList.ItemsSource = Items.Join(types, i => i.AgentTypeId, a => a.Id, (i, a) => new {
                            Id = i.Id,
                            NameCompany = i.NameCompany,
                            AgentTypeId = a.Id,
                            TypeName = a.TypeName,
                            Adress = i.Adress,
                            INN = i.INN,
                            KPP = i.KPP,
                            Director = i.Director,
                            Phone = i.Phone,
                            Email = i.Email,
                            Logo = i.Logo,
                            Priority = i.Priority,
                            Discount = i.Discount
                        }).Where(t => t.TypeName.Contains("ЗАО"));
                        break;
                    case 2:
                        agentsList.ItemsSource = Items.Join(types, i => i.AgentTypeId, a => a.Id, (i, a) => new {
                            Id = i.Id,
                            NameCompany = i.NameCompany,
                            AgentTypeId = a.Id,
                            TypeName = a.TypeName,
                            Adress = i.Adress,
                            INN = i.INN,
                            KPP = i.KPP,
                            Director = i.Director,
                            Phone = i.Phone,
                            Email = i.Email,
                            Logo = i.Logo,
                            Priority = i.Priority,
                            Discount = i.Discount
                        }).Where(t => t.TypeName.Contains("МКК"));
                        break;
                    case 3:
                        agentsList.ItemsSource = Items.Join(types, i => i.AgentTypeId, a => a.Id, (i, a) => new {
                            Id = i.Id,
                            NameCompany = i.NameCompany,
                            AgentTypeId = a.Id,
                            TypeName = a.TypeName,
                            Adress = i.Adress,
                            INN = i.INN,
                            KPP = i.KPP,
                            Director = i.Director,
                            Phone = i.Phone,
                            Email = i.Email,
                            Logo = i.Logo,
                            Priority = i.Priority,
                            Discount = i.Discount
                        }).Where(t => t.TypeName.Contains("МФО"));
                        break;
                    case 4:
                        agentsList.ItemsSource = Items.Join(types, i => i.AgentTypeId, a => a.Id, (i, a) => new {
                            Id = i.Id,
                            NameCompany = i.NameCompany,
                            AgentTypeId = a.Id,
                            TypeName = a.TypeName,
                            Adress = i.Adress,
                            INN = i.INN,
                            KPP = i.KPP,
                            Director = i.Director,
                            Phone = i.Phone,
                            Email = i.Email,
                            Logo = i.Logo,
                            Priority = i.Priority,
                            Discount = i.Discount
                        }).Where(t => t.TypeName.Contains("ОАО"));
                        break;
                    case 5:
                        agentsList.ItemsSource = Items.Join(types, i => i.AgentTypeId, a => a.Id, (i, a) => new {
                            Id = i.Id,
                            NameCompany = i.NameCompany,
                            AgentTypeId = a.Id,
                            TypeName = a.TypeName,
                            Adress = i.Adress,
                            INN = i.INN,
                            KPP = i.KPP,
                            Director = i.Director,
                            Phone = i.Phone,
                            Email = i.Email,
                            Logo = i.Logo,
                            Priority = i.Priority,
                            Discount = i.Discount
                        }).Where(t => t.TypeName.Contains("ООО"));
                        break;
                    case 6:
                        agentsList.ItemsSource = Items.Join(types, i => i.AgentTypeId, a => a.Id, (i, a) => new {
                            Id = i.Id,
                            NameCompany = i.NameCompany,
                            AgentTypeId = a.Id,
                            TypeName = a.TypeName,
                            Adress = i.Adress,
                            INN = i.INN,
                            KPP = i.KPP,
                            Director = i.Director,
                            Phone = i.Phone,
                            Email = i.Email,
                            Logo = i.Logo,
                            Priority = i.Priority,
                            Discount = i.Discount
                        }).Where(t => t.TypeName.Contains("ПАО"));
                        break;
                }
            }
        }

        private void agentsList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Agents agent = new Agents();
            agent = agentsList.SelectedValue as Agents;
            AddAgent addAgent = new AddAgent(agent);
            addAgent.Owner = this;
            addAgent.Show();
            Hide();
        }

        private void addAgent_Click(object sender, RoutedEventArgs e)
        {
            AddAgent addAgent = new AddAgent();
            addAgent.Owner = this;
            addAgent.Show();
            Hide();
        }
    }
}
