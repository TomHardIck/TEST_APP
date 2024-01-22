using System;
using System.Collections.Generic;
using System.Data;
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
using Test_App.Stock_TESTDataSetTableAdapters;

namespace Test_App
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ProductViewTableAdapter viewProductAdapter = new ProductViewTableAdapter();
        IncomingViewTableAdapter viewIncomingAdapter = new IncomingViewTableAdapter();
        IncomingTableAdapter incomingAdapter = new IncomingTableAdapter();
        ProductTableAdapter productsAdapter = new ProductTableAdapter();
        ConsumptionTableAdapter consumptionAdapter = new ConsumptionTableAdapter();
        ConsumptionViewTableAdapter viewConsumptionAdapter = new ConsumptionViewTableAdapter();
        public MainWindow()
        {
            InitializeComponent();
            UpdateData();
        }


        /// <summary>
        /// Обновляет данные DataGrid-ов и ComboBox-ов
        /// </summary>
        private void UpdateData()
        {
            productGrid.ItemsSource = viewProductAdapter.GetData();
            incomingGrid.ItemsSource = viewIncomingAdapter.GetData();
            consGrid.ItemsSource = viewConsumptionAdapter.GetData();
            productCons.ItemsSource = productsAdapter.GetData().Select(x => x.Product_Name);
            productIncoming.ItemsSource = productsAdapter.GetData().Select(x => x.Product_Name);
        }


        /// <summary>
        /// Проверка на уникальность названия товара в БД
        /// </summary>
        /// <param name="nameToCheck"></param>
        /// <returns>Возвращает TRUE или FALSE, в зависимости от того существует ли уже это название в БД</returns>
        private bool CheckUniqueProduct(string nameToCheck)
        {
            if(productsAdapter.GetData().Where(x => x.Product_Name.Equals(nameToCheck)).FirstOrDefault() != null)
            {
                return false;
            }
            return true;
        }

        private void addButton_Click(object sender, RoutedEventArgs e) //добавление товара
        {
            if (!string.IsNullOrEmpty(nameProduct.Text))
            {
                if(!CheckUniqueProduct(nameProduct.Text))
                {
                    MessageBox.Show("Товар с таким наименованием уже существует! В добавлении отказано.");
                }
                else
                {
                    productsAdapter.Insert(nameProduct.Text);
                    UpdateData();
                    MessageBox.Show("Успешное добавление нового товара!");
                }
            }
            else
            {
                MessageBox.Show("Заполните поле для добавления товара!");
            }
            UpdateData();
        }

        private void productGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) //заполнение полей с данными о товаре
        {
            try
            {
                DataRowView selectedRow = (DataRowView)productGrid.SelectedItem;
                if (selectedRow != null)
                {
                    nameProduct.Text = selectedRow[0].ToString();
                }
                else
                {

                }
            }
            catch(Exception ex)
            {

            }
        }

        private void delButton_Click(object sender, RoutedEventArgs e) //удаление товара
        {
            try
            {
                DataRowView selectedRow = (DataRowView)productGrid.SelectedItem;
                if (selectedRow != null)
                {
                    int idToDelete = productsAdapter.GetData().Where(x => x.Product_Name.Equals(selectedRow[0].ToString())).FirstOrDefault().ID_Product;
                    productsAdapter.Delete(idToDelete, selectedRow[0].ToString());
                    nameProduct.Text = "";
                    UpdateData();
                    MessageBox.Show($"Успешное удаление товара с ID: {idToDelete}");
                }
                else
                {
                    MessageBox.Show("Выберите запись в таблице чтобы удалить ее!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void editButton_Click(object sender, RoutedEventArgs e) //изменение товара
        {
            try
            {
                DataRowView selectedRow = (DataRowView)productGrid.SelectedItem;
                if (selectedRow != null && !string.IsNullOrEmpty(nameProduct.Text))
                {
                    if (CheckUniqueProduct(nameProduct.Text))
                    {
                        int idToUpdate = productsAdapter.GetData().Where(x => x.Product_Name.Equals(selectedRow[0].ToString())).FirstOrDefault().ID_Product;
                        productsAdapter.Update(nameProduct.Text, idToUpdate, selectedRow[0].ToString());
                        nameProduct.Text = "";
                        UpdateData();
                        MessageBox.Show($"Успешное изменение товара с ID: {idToUpdate}");
                    }
                    else
                    {
                        MessageBox.Show("Данный товар уже существует! Попробуйте изменить название.");
                    }
                }
                else
                {
                    MessageBox.Show("Выберите запись в таблице и заполните текстовое поле чтобы изменить товар!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void addIncomingButton_Click(object sender, RoutedEventArgs e) //добавление прихода
        {
            try
            {
                if (!string.IsNullOrEmpty(quantityIncoming.Text) && productIncoming.SelectedItem != null && dateIncoming.SelectedDate != null)
                {
                    int idToInsert = productsAdapter.GetData().Where(x => x.Product_Name.Equals(productIncoming.SelectedItem.ToString())).FirstOrDefault().ID_Product;
                    incomingAdapter.Insert(dateIncoming.SelectedDate.Value.Date, int.Parse(quantityIncoming.Text), idToInsert);
                    UpdateData();
                    MessageBox.Show("Приход успешно добавлен!");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void incomingGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) //заполнение полей в окне при изменении выбранной записи
        {
            try
            {
                DataRowView selectedRow = (DataRowView)incomingGrid.SelectedItem;
                if (selectedRow != null)
                {
                    dateIncoming.SelectedDate = DateTime.Parse(selectedRow[0].ToString());
                    quantityIncoming.Text = selectedRow[1].ToString();
                    productIncoming.SelectedItem = selectedRow[2].ToString();
                }
                else
                {

                }
            }
            catch (Exception ex)
            {

            }
        }

        private void filterButton_Click(object sender, RoutedEventArgs e) //фильтрация данных. в зависимости от кол-ва выбранных параметров осуществляем выборку данных
        {
            if(dateStartIncoming.SelectedDate != null && dateFinishIncoming.SelectedDate != null)
            {
                var filteredData = viewIncomingAdapter.GetData().Where(x => x.Дата_прихода > dateStartIncoming.SelectedDate && x.Дата_прихода < dateFinishIncoming.SelectedDate);
                if (filteredData.Count() > 0)
                {
                    incomingGrid.ItemsSource = filteredData.CopyToDataTable().DefaultView;
                }
                else
                {
                    MessageBox.Show("По данным параметрам не найдено данных! Попробуйте изменить фильтр.");
                }
            }
            else if (dateStartIncoming.SelectedDate != null && dateFinishIncoming.SelectedDate == null)
            {
                var filteredData = viewIncomingAdapter.GetData().Where(x => x.Дата_прихода > dateStartIncoming.SelectedDate);
                if (filteredData.Count() > 0)
                {
                    incomingGrid.ItemsSource = filteredData.CopyToDataTable().DefaultView;
                }
                else
                {
                    MessageBox.Show("По данным параметрам не найдено данных! Попробуйте изменить фильтр.");
                }
            }
            else if (dateStartIncoming.SelectedDate == null && dateFinishIncoming.SelectedDate != null)
            {
                var filteredData = viewIncomingAdapter.GetData().Where(x => x.Дата_прихода < dateFinishIncoming.SelectedDate);
                if (filteredData.Count() > 0)
                {
                    incomingGrid.ItemsSource = filteredData.CopyToDataTable().DefaultView;
                }
                else
                {
                    MessageBox.Show("По данным параметрам не найдено данных! Попробуйте изменить фильтр.");
                }
            }
        }

        private void clearFilter_Click(object sender, RoutedEventArgs e) //очистка фильтрации прихода
        {
            incomingGrid.ItemsSource = viewIncomingAdapter.GetData();
            dateStartIncoming.SelectedDate = null;
            dateFinishIncoming.SelectedDate = null;
        }

        private void delIncomingButton_Click(object sender, RoutedEventArgs e) //удаление прихода
        {
            try
            {
                DataRowView selectedRow = (DataRowView)incomingGrid.SelectedItem; //получаем выбранную запись
                if (selectedRow != null)
                {
                    //получаем внешний ключ
                    int idRecordProductToDelete = productsAdapter.GetData().Where(x => x.Product_Name.Equals(selectedRow[2].ToString())).FirstOrDefault().ID_Product;
                    //получаем запись, которую нужно удалить
                    var rowToDelete = incomingAdapter.GetData().Where(x =>
                        x.Date_Incoming.Equals(DateTime.Parse(selectedRow[0].ToString())) &&
                        x.Quantity_Incoming.Equals(int.Parse(selectedRow[1].ToString())) &&
                        x.Product_ID.Equals(idRecordProductToDelete)).FirstOrDefault();
                    //удаляем запись
                    incomingAdapter.Delete(rowToDelete.ID_Incoming, rowToDelete.Date_Incoming, rowToDelete.Quantity_Incoming, rowToDelete.Product_ID);
                    UpdateData();
                    MessageBox.Show("Приход успешно удален!");
                }
                else
                {
                    MessageBox.Show("Выберите запись для удаления!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void editIncomingButton_Click(object sender, RoutedEventArgs e) //изменяем запись
        {
            try
            {
                DataRowView selectedRow = (DataRowView)incomingGrid.SelectedItem; //получаем выбранную запись
                if (selectedRow != null)
                {
                    //получаем внешний ключ
                    int idRecordProductToUpdate = productsAdapter.GetData().Where(x => x.Product_Name.Equals(selectedRow[2].ToString())).FirstOrDefault().ID_Product;

                    //получаем новый ID, который впоследствии будет вставлен в БД
                    int idNewProduct = productsAdapter.GetData().Where(x => x.Product_Name.Equals(productIncoming.SelectedItem.ToString())).FirstOrDefault().ID_Product;

                    //получаем запись, которую нужно изменить
                    var rowToUpdate = incomingAdapter.GetData().Where(x =>
                        x.Date_Incoming.Equals(DateTime.Parse(selectedRow[0].ToString())) &&
                        x.Quantity_Incoming.Equals(int.Parse(selectedRow[1].ToString())) &&
                        x.Product_ID.Equals(idRecordProductToUpdate)).FirstOrDefault();
                    //изменяем запись
                    incomingAdapter.Update(dateIncoming.SelectedDate.Value.Date, int.Parse(quantityIncoming.Text), idNewProduct, rowToUpdate.ID_Incoming, rowToUpdate.Date_Incoming, rowToUpdate.Quantity_Incoming, rowToUpdate.Product_ID);
                    UpdateData();
                    MessageBox.Show("Приход успешно изменен!");
                }
                else
                {
                    MessageBox.Show("Выберите запись для изменения!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void addConsButton_Click(object sender, RoutedEventArgs e) //добавление расхода
        {
            try
            {
                if (!string.IsNullOrEmpty(quantityCons.Text) && productCons.SelectedItem != null && dateCons.SelectedDate != null)
                {
                    int idToInsert = productsAdapter.GetData().Where(x => x.Product_Name.Equals(productCons.SelectedItem.ToString())).FirstOrDefault().ID_Product;
                    consumptionAdapter.Insert(dateCons.SelectedDate.Value.Date, int.Parse(quantityCons.Text), idToInsert);
                    UpdateData();
                    MessageBox.Show("Расход успешно добавлен!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void consGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) //заполнение полей данными о расходе
        {
            try
            {
                DataRowView selectedRow = (DataRowView)consGrid.SelectedItem;
                if (selectedRow != null)
                {
                    dateCons.SelectedDate = DateTime.Parse(selectedRow[0].ToString());
                    quantityCons.Text = selectedRow[1].ToString();
                    productCons.SelectedItem = selectedRow[2].ToString();
                }
                else
                {

                }
            }
            catch (Exception ex)
            {

            }
        }

        private void delConsButton_Click(object sender, RoutedEventArgs e) //удаление расхода
        {
            try
            {
                DataRowView selectedRow = (DataRowView)consGrid.SelectedItem; //получаем выбранную запись
                if (selectedRow != null)
                {
                    //получаем внешний ключ
                    int idRecordProductToDelete = productsAdapter.GetData().Where(x => x.Product_Name.Equals(selectedRow[2].ToString())).FirstOrDefault().ID_Product;
                    //получаем запись, которую нужно удалить
                    var rowToDelete = consumptionAdapter.GetData().Where(x =>
                        x.Date_Consumption.Equals(DateTime.Parse(selectedRow[0].ToString())) &&
                        x.Quantity_Consumption.Equals(int.Parse(selectedRow[1].ToString())) &&
                        x.Product_ID.Equals(idRecordProductToDelete)).FirstOrDefault();
                    //удаляем запись
                    consumptionAdapter.Delete(rowToDelete.ID_Consumption, rowToDelete.Date_Consumption, rowToDelete.Quantity_Consumption, rowToDelete.Product_ID);
                    UpdateData();
                    MessageBox.Show("Расход успешно удален!");
                }
                else
                {
                    MessageBox.Show("Выберите запись для удаления!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void editConsButton_Click(object sender, RoutedEventArgs e) //изменение расхода
        {
            try
            {
                DataRowView selectedRow = (DataRowView)consGrid.SelectedItem; //получаем выбранную запись
                if (selectedRow != null)
                {
                    //получаем внешний ключ
                    int idRecordProductToUpdate = productsAdapter.GetData().Where(x => x.Product_Name.Equals(selectedRow[2].ToString())).FirstOrDefault().ID_Product;

                    //получаем новый ID, который впоследствии будет вставлен в БД
                    int idNewProduct = productsAdapter.GetData().Where(x => x.Product_Name.Equals(productCons.SelectedItem.ToString())).FirstOrDefault().ID_Product;

                    //получаем запись, которую нужно изменить
                    var rowToUpdate = consumptionAdapter.GetData().Where(x =>
                        x.Date_Consumption.Equals(DateTime.Parse(selectedRow[0].ToString())) &&
                        x.Quantity_Consumption.Equals(int.Parse(selectedRow[1].ToString())) &&
                        x.Product_ID.Equals(idRecordProductToUpdate)).FirstOrDefault();
                    //изменяем запись
                    consumptionAdapter.Update(dateCons.SelectedDate.Value.Date, int.Parse(quantityCons.Text), idNewProduct, rowToUpdate.ID_Consumption, rowToUpdate.Date_Consumption, rowToUpdate.Quantity_Consumption, rowToUpdate.Product_ID);
                    UpdateData();
                    MessageBox.Show("Расход успешно изменен!");
                }
                else
                {
                    MessageBox.Show("Выберите запись для изменения!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void filterButtonCons_Click(object sender, RoutedEventArgs e) //фильтрация данных о расходах
        {
            if (dateStartCons.SelectedDate != null && dateFinishCons.SelectedDate != null)
            {
                var filteredData = viewConsumptionAdapter.GetData().Where(x => x.Дата_расхода > dateStartCons.SelectedDate && x.Дата_расхода < dateFinishCons.SelectedDate);
                if(filteredData.Count() > 0)
                {
                    consGrid.ItemsSource = filteredData.CopyToDataTable().DefaultView;
                }
                else
                {
                    MessageBox.Show("По данным параметрам не найдено данных! Попробуйте изменить фильтр.");
                }
            }
            else if (dateStartCons.SelectedDate != null && dateFinishCons.SelectedDate == null)
            {
                var filteredData = viewConsumptionAdapter.GetData().Where(x => x.Дата_расхода > dateStartCons.SelectedDate);
                if (filteredData.Count() > 0)
                {
                    consGrid.ItemsSource = filteredData.CopyToDataTable().DefaultView;
                }
                else
                {
                    MessageBox.Show("По данным параметрам не найдено данных! Попробуйте изменить фильтр.");
                }
            }
            else if (dateStartCons.SelectedDate == null && dateFinishCons.SelectedDate != null)
            {
                var filteredData = viewConsumptionAdapter.GetData().Where(x => x.Дата_расхода < dateFinishCons.SelectedDate);
                if (filteredData.Count() > 0)
                {
                    consGrid.ItemsSource = filteredData.CopyToDataTable().DefaultView;
                }
                else
                {
                    MessageBox.Show("По данным параметрам не найдено данных! Попробуйте изменить фильтр.");
                }
            }
        }

        private void clearFilterCons_Click(object sender, RoutedEventArgs e) //очистка фильтра расходов
        {
            consGrid.ItemsSource = viewConsumptionAdapter.GetData();
            dateStartCons.SelectedDate = null;
            dateFinishCons.SelectedDate = null;
        }
    }
}
