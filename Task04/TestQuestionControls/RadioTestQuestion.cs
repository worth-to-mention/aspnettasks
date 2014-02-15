using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TestQuestionControls
{
    public class RadioTestQuestion : TestQuestion
    {
        private readonly RadioButtonList optionsList;

        public RadioTestQuestion()
        {
            optionsList = new RadioButtonList();
        }

        public ListItem SelectedItem
        {
            get
            {
                return optionsList.SelectedItem;
            }
        }
        public string AnswerItemId { get; set; }


        public void AddItem(ListItem item)
        {
            optionsList.Items.Add(item);
        }
        public void AddItems(IEnumerable<ListItem> items)
        {
            foreach(var item in items)
            {
                AddItem(item);
            }
        }

        public override bool CheckAnswer()
        {
            return SelectedItem.Value == AnswerItemId;
        }


        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            Controls.Add(optionsList);
        }

    }
}
