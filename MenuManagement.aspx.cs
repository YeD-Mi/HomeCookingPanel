using Google.Cloud.Firestore;
using HomeCookingWebPanel.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace HomeCookingWebPanel
{
    public partial class MenuManagement : System.Web.UI.Page
    {
        FirestoreDb database;
        protected void Page_Load(object sender, EventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"firestore.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            database = FirestoreDb.Create("project-management-22705");
            if (!IsPostBack)
            {
                _ = FillDropDownAsync();
                FetchMenus();
            }
        }

        async Task FillDropDownAsync()
        {
            Query Qref = database.Collection("categories");
            QuerySnapshot snap = await Qref.GetSnapshotAsync();
            foreach (DocumentSnapshot docsnap in snap)
            {
                FoodCategoryModel foodCategory = docsnap.ConvertTo<FoodCategoryModel>();
                if (docsnap.Exists)
                {
                    for (int i=0; i < foodCategory.foodCategories.Count; i++)
                        Ddl_FilterCategory.Items.Add(foodCategory.foodCategories[i]);
                }
            }
        }

        async void FetchMenus()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID");
            table.Columns.Add("Name");
            table.Columns.Add("Category");
            table.Columns.Add("Price");
            table.Columns.Add("Menu Content");
            table.Columns.Add("Rating");
            table.Columns.Add("Preparation Time");
            table.Columns.Add("Creation Date");

            //Haberler koleksiyonu ile bağlantı kuruyoruz.
            Query Qref = database.Collection("recipes").OrderByDescending("creationDate");
            QuerySnapshot snap = await Qref.GetSnapshotAsync();
            foreach (DocumentSnapshot docsnap in snap)
            {
                FoodsModel foods = docsnap.ConvertTo<FoodsModel>();
                if (docsnap.Exists)
                {
                        Data.Instance.CombinedContent = string.Join(", ", foods.malzemeler);
                        Data.Instance.CombinedCategory = string.Join(", ", foods.urunTipi);
                        table.Rows.Add(
                        docsnap.Id,
                        foods.urunAdi,
                        Data.Instance.CombinedCategory,
                        foods.urunPrice + " TL",
                        Data.Instance.CombinedContent,
                        foods.urunPuani,
                        foods.hazirlamaSuresi + " Minute",
                        foods.creationDate.AddHours(3).ToString());
                }
            }
            MenuGrid.DataSource = table;
            MenuGrid.DataBind();
            if (MenuGrid.Rows.Count > 0)

                Lbl_Comment.Text = "You can change your menus with the <span style='font-weight: bold;'>Select</span> button.";
            else
                Lbl_Comment.Text = "No menu found to display. You can add new menu.";
        }

        protected void Btn_Reset_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.RawUrl);
        }

        protected void BtnFilter_Click(object sender, EventArgs e)
        {
             FetchMenusFilter();
        }

        async void FetchMenusFilter()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID");
            table.Columns.Add("Name");
            table.Columns.Add("Category");
            table.Columns.Add("Price");
            table.Columns.Add("Menu Content");
            table.Columns.Add("Rating");
            table.Columns.Add("Preparation Time");
            table.Columns.Add("Creation Date");

            //Haberler koleksiyonu ile bağlantı kuruyoruz.
            Query Qref = database.Collection("recipes").OrderByDescending("creationDate");
            QuerySnapshot snap = await Qref.GetSnapshotAsync();
            foreach (DocumentSnapshot docsnap in snap)
            {
                FoodsModel foods = docsnap.ConvertTo<FoodsModel>();
                if (docsnap.Exists)
                {
                    for (int i=0;i<foods.urunTipi.Count;i++)
                    {
                        if(Ddl_FilterCategory.SelectedValue == foods.urunTipi[i])
                        {
                            Data.Instance.CombinedContent = string.Join(", ", foods.malzemeler);
                            Data.Instance.CombinedCategory = string.Join(", ", foods.urunTipi);
                            table.Rows.Add(
                            docsnap.Id,
                            foods.urunAdi,
                            Data.Instance.CombinedCategory,
                            foods.urunPrice + " TL",
                            Data.Instance.CombinedContent,
                            foods.urunPuani,
                            foods.hazirlamaSuresi + " Minute",
                            foods.creationDate.AddHours(3).ToString());
                        }
                    }
                }
            }
            MenuGrid.DataSource = table;
            MenuGrid.DataBind();
            if (MenuGrid.Rows.Count > 0)

                Lbl_Comment.Text = "You can change your menus with the <span style='font-weight: bold;'>Select</span> button.";
            else
                Lbl_Comment.Text = "No menu found to display. You can add new menu.";
        }

        protected void Btn_Edit_Click(object sender, EventArgs e)
        {
                EditIslem();
        }
        async void EditIslem()
        {
            DocumentReference docref = database.Collection("recipes").Document(Data.Instance.MenuProcess);
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"orderStatus",Dd_EditOrderStatu.SelectedValue },
            };
            DocumentSnapshot snap = await docref.GetSnapshotAsync();
            if (snap.Exists)
            {
                await docref.UpdateAsync(data);
            }
            ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "swal({title: 'Changes made to the order are saved.', icon: 'success', button: 'Tamam'}).then(function() {window.location.href = '" + Request.RawUrl + "';});", true);
        }

        protected void MenuGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument);
            GridViewRow row = MenuGrid.Rows[rowIndex];
            if (e.CommandName == "Select")
            {
                Data.Instance.MenuProcess = row.Cells[0].Text;
                ShowEditPage();
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "", "showEdit();", true);
            }
        }
        async void ShowEditPage()
        {
            //Document içine girmesini istersek;
            DocumentReference docref = database.Collection("recipes").Document(Data.Instance.MenuProcess);
            DocumentSnapshot snap = await docref.GetSnapshotAsync();
            OrderModel order = snap.ConvertTo<OrderModel>();
            Txt_EditOrderer.Text = order.userID;
            Txt_EditOrderDate.Text = order.orderDate.AddHours(3).ToString();
            Txt_EditOrderContent.Text = Data.Instance.CombinedOrder;
            Txt_EditAddress.Text = order.userAdress;
            Txt_EditOrderPrice.Text = order.orderPrice;
            Dd_EditOrderStatu.SelectedValue = order.orderStatus;
        }

        protected void MenuGridGrid_RowCreated(object sender, GridViewRowEventArgs e)
        {
            GridViewRow row = e.Row;
            TableCell processRow = row.Cells[0];
            TableCell idrow = row.Cells[1];
            idrow.Visible = false;
            row.Cells.Remove(processRow);
            row.Cells.Add(processRow);
        }
    }
}