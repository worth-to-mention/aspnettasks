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
    public class SelectTestQuestion : TestQuestion
    {
        private readonly CheckBoxList optionsList;
        private readonly List<string> answers;

        public SelectTestQuestion()
        {
            optionsList = new CheckBoxList();
            answers = new List<string>();
        }
        

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
        public IEnumerable<ListItem> GetSelectedItems()
        {
            foreach(ListItem item in optionsList.Items)
            {
                if (item.Selected)
                    yield return item;
            }
        }
        public void AddAnswerId(string id)
        {
            answers.Add(id);
        }
        public void AddAnswerIds(IEnumerable<string> ids)
        {
            foreach(var id in ids)
            {
                AddAnswerId(id);
            }
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            Controls.Add(optionsList);
        }

        public override bool CheckAnswer()
        {
            var selectedItems = GetSelectedItems().ToList();
            if (selectedItems.Count != answers.Count
                || selectedItems.Count == 0)
                return false;

            return answers.All(answ =>
            {
                foreach(var item in GetSelectedItems())
                {
                    if (item.Value == answ)
                        return true;
                }
                return false;
            });
            
        }
    }
}
