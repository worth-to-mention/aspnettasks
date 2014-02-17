using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;

namespace TestQuestionControls
{
    /// <summary>
    /// Represents test question with radio button options.
    /// </summary>
    public class RadioTestQuestion : TestQuestion
    {
        private readonly RadioButtonList optionsList;

        /// <summary>
        /// Constructs instance of RadioTestQuestion class.
        /// </summary>
        public RadioTestQuestion()
        {
            optionsList = new RadioButtonList();
        }
        /// <summary>
        /// Gets selected item from radio button list.
        /// </summary>
        public ListItem SelectedItem
        {
            get
            {
                return optionsList.SelectedItem;
            }
        }
        /// <summary>
        /// Gets or sets answer id.
        /// Answer id is a value property of one of the
        /// items from radio button list.
        /// </summary>
        public string AnswerItemId { get; set; }

        /// <summary>
        /// Adds item to the radio button list.
        /// </summary>
        /// <param name="item">Item to add to the radio button list.</param>
        public void AddItem(ListItem item)
        {
            optionsList.Items.Add(item);
        }
        /// <summary>
        /// Adds items to the radio button list.
        /// </summary>
        /// <param name="items">Items to add to the radio button list.</param>
        public void AddItems(IEnumerable<ListItem> items)
        {
            foreach(var item in items)
            {
                AddItem(item);
            }
        }
        /// <summary>
        /// Checks whether selected item value of the radio button list
        /// is the same as the value in AsnwerItemId.
        /// </summary>
        /// <returns>True if values are equal.</returns>
        public override bool CheckAnswer()
        {
            if (SelectedItem == null)
                return false;
            return SelectedItem.Value == AnswerItemId;
        }


        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            Controls.Add(optionsList);

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

        }

    }
}
