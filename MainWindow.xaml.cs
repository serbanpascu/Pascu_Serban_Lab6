using AutoLotModel;
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
using System.Data.Entity;
using System.Data;

namespace Pascu_Serban_Lab6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing
    }
    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;
        AutoLotEntitiesModel ctx = new AutoLotEntitiesModel();

        CollectionViewSource customerViewSource;
        Binding txtFirstNameBinding = new Binding();
        Binding txtLastNameBinding = new Binding();

        CollectionViewSource inventoryViewSource;
        Binding txtColorBinding = new Binding();
        Binding txtMakeBinding = new Binding();

        CollectionViewSource customerOrdersViewSource;
        Binding txtCustomer = new Binding();
        Binding txtInventory = new Binding();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            txtFirstNameBinding.Path = new PropertyPath("FirstName");
            txtLastNameBinding.Path = new PropertyPath("LastName");
            txtColorBinding.Path = new PropertyPath("Color");
            txtMakeBinding.Path = new PropertyPath("Make");
            txtCustomer.Path = new PropertyPath("Customer");
            txtInventory.Path = new PropertyPath("Inventory");

            firstNameTextBox.SetBinding(TextBox.TextProperty, txtFirstNameBinding);
            lastNameTextBox.SetBinding(TextBox.TextProperty, txtLastNameBinding);
            colorTextBox.SetBinding(TextBox.TextProperty, txtColorBinding);
            makeTextBox.SetBinding(TextBox.TextProperty, txtMakeBinding);
            cmbCustomer.SetBinding(ComboBox.TextProperty, txtCustomer);
            cmbInventory.SetBinding(ComboBox.TextProperty, txtInventory);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //using System.Data.Entity;
            customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            customerViewSource.Source = ctx.Customers.Local;
            customerOrdersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerOrdersViewSource")));
            //customerOrdersViewSource.Source = ctx.Orders.Local;
            ctx.Customers.Load();
            ctx.Orders.Load();

            System.Windows.Data.CollectionViewSource inventoryViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("inventoryViewSource")));
            inventoryViewSource.Source = ctx.Inventories.Local;
            ctx.Inventories.Load();

            cmbCustomer.ItemsSource = ctx.Customers.Local;
            //cmbCustomer.DisplayMemberPath = "FirstName";
            cmbCustomer.SelectedValuePath = "CustId";

            cmbInventory.ItemsSource = ctx.Inventories.Local;
            //cmbInventory.DisplayMemberPath = "Make";
            cmbInventory.SelectedValuePath = "CardId";

            BindDataGrid();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem Customer entity
                    customer = new Customer()
                    {
                        FirstName = firstNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim()
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Customers.Add(customer);
                    customerViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                //using System.Data;
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                customerDataGrid.IsEnabled = true;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
            }
            else if (action == ActionState.Edit)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    customer.FirstName = firstNameTextBox.Text.Trim();
                    customer.LastName = lastNameTextBox.Text.Trim();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();
                //pozitionarea pe item-ul curent
                customerViewSource.View.MoveCurrentTo(customer);
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;

                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                customerDataGrid.IsEnabled = true;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    ctx.Customers.Remove(customer);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;

                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                customerDataGrid.IsEnabled = true;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;

                firstNameTextBox.SetBinding(TextBox.TextProperty, txtFirstNameBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, txtLastNameBinding);     
            }
            SetValidationBinding();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToNext();
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNew_click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;

            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            customerDataGrid.IsEnabled = false;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;

            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            firstNameTextBox.Text = "";
            lastNameTextBox.Text = "";
            Keyboard.Focus(firstNameTextBox);
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;

            string tempFirstName = firstNameTextBox.Text.ToString();
            string tempLastName = lastNameTextBox.Text.ToString();

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;

            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            customerDataGrid.IsEnabled = false;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            SetValidationBinding();
            firstNameTextBox.Text = tempFirstName;
            lastNameTextBox.Text = tempLastName;
            Keyboard.Focus(firstNameTextBox);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;

            string tempFirstName = firstNameTextBox.Text.ToString();
            string tempLastName = lastNameTextBox.Text.ToString();

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;

            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            customerDataGrid.IsEnabled = false;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            firstNameTextBox.Text = tempFirstName;
            lastNameTextBox.Text = tempLastName;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNew.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;
            customerDataGrid.IsEnabled = true;
            btnPrev.IsEnabled = true;
            btnNext.IsEnabled = true;

            firstNameTextBox.IsEnabled = false;
            lastNameTextBox.IsEnabled = false;

            firstNameTextBox.SetBinding(TextBox.TextProperty, txtFirstNameBinding);
            lastNameTextBox.SetBinding(TextBox.TextProperty, txtLastNameBinding);
        }

        private void btnNew2_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNew2.IsEnabled = false;
            btnEdit2.IsEnabled = false;
            btnDelete2.IsEnabled = false;

            btnSave2.IsEnabled = true;
            btnCancel2.IsEnabled = true;
            inventoryDataGrid.IsEnabled = false;
            btnPrev2.IsEnabled = false;
            btnNext2.IsEnabled = false;

            colorTextBox.IsEnabled = true;
            makeTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            colorTextBox.Text = "";
            makeTextBox.Text = "";
            Keyboard.Focus(colorTextBox);

        }

        private void btnEdit2_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;

            string tempColor = colorTextBox.Text.ToString();
            string tempMake = makeTextBox.Text.ToString();

            btnNew2.IsEnabled = false;
            btnEdit2.IsEnabled = false;
            btnDelete2.IsEnabled = false;

            btnSave2.IsEnabled = true;
            btnCancel2.IsEnabled = true;
            inventoryDataGrid.IsEnabled = false;
            btnPrev2.IsEnabled = false;
            btnNext2.IsEnabled = false;
            colorTextBox.IsEnabled = true;
            makeTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            colorTextBox.Text = tempColor;
            makeTextBox.Text = tempMake;
            Keyboard.Focus(colorTextBox);
        }

        private void btnDelete2_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;

            string tempColor = colorTextBox.Text.ToString();
            string tempMake = makeTextBox.Text.ToString();

            btnNew2.IsEnabled = false;
            btnEdit2.IsEnabled = false;
            btnDelete2.IsEnabled = false;

            btnSave2.IsEnabled = true;
            btnCancel2.IsEnabled = true;
            inventoryDataGrid.IsEnabled = false;
            btnPrev2.IsEnabled = false;
            btnNext2.IsEnabled = false;

            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            colorTextBox.Text = tempColor;
            makeTextBox.Text = tempMake;
        }

        private void btnSave2_Click(object sender, RoutedEventArgs e)
        {
            Inventory inventory = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem Customer entity
                    inventory = new Inventory()
                    {
                        Color = colorTextBox.Text.Trim(),
                        Make = makeTextBox.Text.Trim()
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Inventories.Add(inventory);
                    inventoryViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                //using System.Data;
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew2.IsEnabled = true;
                btnEdit2.IsEnabled = true;
                btnSave2.IsEnabled = false;
                btnCancel2.IsEnabled = false;
                inventoryDataGrid.IsEnabled = true;
                btnPrev2.IsEnabled = true;
                btnNext2.IsEnabled = true;
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;
            }
            else if (action == ActionState.Edit)
            {
                try
                {
                    inventory = (Inventory)inventoryDataGrid.SelectedItem;
                    inventory.Color = colorTextBox.Text.Trim();
                    inventory.Make = makeTextBox.Text.Trim();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                inventoryViewSource.View.Refresh();
                //pozitionarea pe item-ul curent
                inventoryViewSource.View.MoveCurrentTo(inventory);
                btnNew2.IsEnabled = true;
                btnEdit2.IsEnabled = true;
                btnDelete2.IsEnabled = true;

                btnSave2.IsEnabled = false;
                btnCancel2.IsEnabled = false;
                inventoryDataGrid.IsEnabled = true;
                btnPrev2.IsEnabled = true;
                btnNext2.IsEnabled = true;
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    inventory = (Inventory)inventoryDataGrid.SelectedItem;
                    ctx.Inventories.Remove(inventory);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                inventoryViewSource.View.Refresh();
                btnNew2.IsEnabled = true;
                btnEdit2.IsEnabled = true;
                btnDelete2.IsEnabled = true;

                btnSave2.IsEnabled = false;
                btnCancel2.IsEnabled = false;
                inventoryDataGrid.IsEnabled = true;
                btnPrev2.IsEnabled = true;
                btnNext2.IsEnabled = true;
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;

                colorTextBox.SetBinding(TextBox.TextProperty, txtColorBinding);
                makeTextBox.SetBinding(TextBox.TextProperty, txtMakeBinding);
            }
        }

        private void btnCancel2_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNew2.IsEnabled = true;
            btnEdit2.IsEnabled = true;
            btnSave2.IsEnabled = false;
            btnCancel2.IsEnabled = false;
            inventoryDataGrid.IsEnabled = true;
            btnPrev2.IsEnabled = true;
            btnNext2.IsEnabled = true;

            colorTextBox.IsEnabled = false;
            makeTextBox.IsEnabled = false;

            colorTextBox.SetBinding(TextBox.TextProperty, txtColorBinding);
            makeTextBox.SetBinding(TextBox.TextProperty, txtMakeBinding);
        }

        private void btnPrev2_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNext2_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToNext();
        }

        private void btnNew3_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNew3.IsEnabled = false;
            btnEdit3.IsEnabled = false;
            btnDelete3.IsEnabled = false;

            btnSave3.IsEnabled = true;
            btnCancel3.IsEnabled = true;
            ordersDataGrid.IsEnabled = false;
            btnPrev3.IsEnabled = false;
            btnNext3.IsEnabled = false;

            cmbCustomer.IsEnabled = true;
            cmbInventory.IsEnabled = true;

            BindingOperations.ClearBinding(cmbCustomer, ComboBox.TextProperty);
            BindingOperations.ClearBinding(cmbInventory, ComboBox.TextProperty);
            cmbCustomer.Text = "";
            cmbInventory.Text = "";
            Keyboard.Focus(cmbCustomer);
        }

        private void btnEdit3_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;

            string tempCustomer = cmbCustomer.Text.ToString();
            string tempInventory = cmbInventory.Text.ToString();

            btnNew3.IsEnabled = false;
            btnEdit3.IsEnabled = false;
            btnDelete3.IsEnabled = false;

            btnSave3.IsEnabled = true;
            btnCancel3.IsEnabled = true;
            ordersDataGrid.IsEnabled = false;
            btnPrev3.IsEnabled = false;
            btnNext3.IsEnabled = false;
            cmbCustomer.IsEnabled = true;
            cmbInventory.IsEnabled = true;

            BindingOperations.ClearBinding(cmbCustomer, ComboBox.TextProperty);
            BindingOperations.ClearBinding(cmbInventory, ComboBox.TextProperty);
            cmbCustomer.Text = tempCustomer;
            cmbInventory.Text = tempInventory;
            Keyboard.Focus(cmbCustomer);
        }

        private void btnDelete3_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;

            string tempCustomer = cmbCustomer.Text.ToString();
            string tempInventory = cmbInventory.Text.ToString();

            btnNew3.IsEnabled = false;
            btnEdit3.IsEnabled = false;
            btnDelete3.IsEnabled = false;

            btnSave3.IsEnabled = true;
            btnCancel3.IsEnabled = true;
            ordersDataGrid.IsEnabled = false;
            btnPrev3.IsEnabled = false;
            btnNext3.IsEnabled = false;

            BindingOperations.ClearBinding(cmbCustomer, ComboBox.TextProperty);
            BindingOperations.ClearBinding(cmbInventory, ComboBox.TextProperty);
            cmbCustomer.Text = tempCustomer;
            cmbInventory.Text = tempInventory;
        }

        private void btnSave3_Click(object sender, RoutedEventArgs e)
        {
            Order order = null;
            if (action == ActionState.New)
            {
                try
                {
                    Customer customer = (Customer)cmbCustomer.SelectedItem;
                    Inventory inventory = (Inventory)cmbInventory.SelectedItem;

                    //instantiem Order entity
                    order = new Order()
                    {
                        CustId = customer.CustId,
                        CardId = inventory.CardId
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Orders.Add(order);
                    customerOrdersViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew3.IsEnabled = true;
                btnEdit3.IsEnabled = true;
                btnSave3.IsEnabled = false;
                btnCancel3.IsEnabled = false;
                ordersDataGrid.IsEnabled = true;
                btnPrev3.IsEnabled = true;
                btnNext3.IsEnabled = true;
                cmbCustomer.IsEnabled = false;
                cmbInventory.IsEnabled = false;
            }
            else if (action == ActionState.Edit)
            {
                dynamic selectedOrder = ordersDataGrid.SelectedItem;
                try
                {
                    int curr_id = selectedOrder.OrderId;

                    var editedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (editedOrder != null)
                    {
                        editedOrder.CustId = Int32.Parse(cmbCustomer.SelectedValue.ToString());
                        editedOrder.CardId = Int32.Parse(cmbInventory.SelectedValue.ToString());
                        ctx.SaveChanges();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                BindDataGrid();
                customerViewSource.View.MoveCurrentTo(selectedOrder);

                customerOrdersViewSource.View.Refresh();
                customerOrdersViewSource.View.MoveCurrentTo(order);
                btnNew3.IsEnabled = true;
                btnEdit3.IsEnabled = true;
                btnDelete3.IsEnabled = true;

                btnSave3.IsEnabled = false;
                btnCancel3.IsEnabled = false;
                ordersDataGrid.IsEnabled = true;
                btnPrev3.IsEnabled = true;
                btnNext3.IsEnabled = true;
                cmbCustomer.IsEnabled = false;
                cmbInventory.IsEnabled = false;

            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    dynamic selectedOrder = ordersDataGrid.SelectedItem;

                    int curr_id = selectedOrder.OrderId;
                    var deletedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (deletedOrder != null)
                    {
                        ctx.Orders.Remove(deletedOrder);
                        ctx.SaveChanges();
                        MessageBox.Show("Order Deleted Sucessfully", "Message");
                        BindDataGrid();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerOrdersViewSource.View.Refresh();
                btnNew3.IsEnabled = true;
                btnEdit3.IsEnabled = true;
                btnDelete3.IsEnabled = true;

                btnSave3.IsEnabled = false;
                btnCancel3.IsEnabled = false;
                ordersDataGrid.IsEnabled = true;
                btnPrev3.IsEnabled = true;
                btnNext3.IsEnabled = true;
                cmbCustomer.IsEnabled = false;
                cmbInventory.IsEnabled = false;

                cmbCustomer.SetBinding(ComboBox.TextProperty, txtCustomer);
                cmbInventory.SetBinding(ComboBox.TextProperty, txtInventory);
            }
        }
        private void btnCancel3_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNew3.IsEnabled = true;
            btnEdit3.IsEnabled = true;
            btnSave3.IsEnabled = false;
            btnCancel3.IsEnabled = false;
            ordersDataGrid.IsEnabled = true;
            btnPrev3.IsEnabled = true;
            btnNext3.IsEnabled = true;

            cmbCustomer.IsEnabled = false;
            cmbInventory.IsEnabled = false;

            cmbCustomer.SetBinding(ComboBox.TextProperty, txtCustomer);
            cmbInventory.SetBinding(ComboBox.TextProperty, txtInventory);
        }

        private void btnPrev3_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNext3_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToNext();
        }

        private void BindDataGrid()
        {
            var queryOrder = from ord in ctx.Orders join cust in ctx.Customers on ord.CustId equals cust.CustId join inv in ctx.Inventories on ord.CardId equals inv.CardId select new { ord.OrderId, ord.CardId, ord.CustId, cust.FirstName, cust.LastName, inv.Make, inv.Color };
            customerOrdersViewSource.Source = queryOrder.ToList();
        }

        private void SetValidationBinding()
        {
            Binding firstNameValidationBinding = new Binding();
            firstNameValidationBinding.Source = customerViewSource;
            firstNameValidationBinding.Path = new PropertyPath("FirstName");
            firstNameValidationBinding.NotifyOnValidationError = true;
            firstNameValidationBinding.Mode = BindingMode.TwoWay;
            firstNameValidationBinding.UpdateSourceTrigger =
           UpdateSourceTrigger.PropertyChanged;
            //string required
            firstNameValidationBinding.ValidationRules.Add(new StringNotEmpty());
            firstNameTextBox.SetBinding(TextBox.TextProperty,
           firstNameValidationBinding);

            Binding lastNameValidationBinding = new Binding();
            lastNameValidationBinding.Source = customerViewSource;
            lastNameValidationBinding.Path = new PropertyPath("LastName");
            lastNameValidationBinding.NotifyOnValidationError = true;
            lastNameValidationBinding.Mode = BindingMode.TwoWay;
            lastNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string min length validator
            lastNameValidationBinding.ValidationRules.Add(new StringMinLengthValidator());
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameValidationBinding); //setare binding nou
        }
    }
}
