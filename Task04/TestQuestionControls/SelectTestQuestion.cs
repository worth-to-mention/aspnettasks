using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TestingSystem.Controls
{
    /// <summary>
    /// Represents test question with multiplie selection
    /// possibility.
    /// </summary>
    [
        ParseChildren(true, "Options"),
        ToolboxData("<{0}:SelectTestQuestion runat=\"server\"> </{0}:SelectTestQuestion>")
    ]
    public class SelectTestQuestion : TestQuestion
    {
        private CheckBoxList optionsList;
        private List<string> answers;

        /// <summary>
        /// Construct an instanse of SelectTestQuestion class.
        /// </summary>
        public SelectTestQuestion()
        {
            optionsList = new CheckBoxList();
            answers = new List<string>();
        }
        
        public ListItemCollection Options
        {
            get
            {
                if (optionsList == null)
                {
                    optionsList = new CheckBoxList();
                }
                return optionsList.Items;
            }
        }

        /// <summary>
        /// Adds item to the checkbox list.
        /// </summary>
        /// <param name="item">Item to add to the checkbox list</param>
        public void AddItem(ListItem item)
        {
            Options.Add(item);
        }
        /// <summary>
        /// Adds items to the checkbox list.
        /// </summary>
        /// <param name="items">Items to add to the checkbox list.</param>
        public void AddItems(IEnumerable<ListItem> items)
        {
            foreach(var item in items)
            {
                AddItem(item);
            }
        }
        /// <summary>
        /// Returns selected items from the checkbox list.
        /// </summary>
        /// <returns>Selected items from the checkbox list.</returns>
        public IEnumerable<ListItem> GetSelectedItems()
        {
            foreach(ListItem item in optionsList.Items)
            {
                if (item.Selected)
                    yield return item;
            }
        }
        /// <summary>
        /// Adds item id of the item that
        /// will be treated as the right answer.
        /// </summary>
        /// <param name="id">
        /// Id of the item that
        /// will be treated as the right answer.
        /// </param>
        public void AddAnswerId(string id)
        {
            answers.Add(id);
        }
        /// <summary>
        /// Adds item ids of the items that
        /// will be treated as the right answers.
        /// </summary>
        /// <param name="ids">
        /// Ids of the items that
        /// will be treated as the right answers.
        /// </param>
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
        /// <summary>
        /// Checks whether selected items from the checkbox list
        /// is with the same id as in the answer list.
        /// </summary>
        /// <returns>
        /// True if selected items is with the same ids
        /// as in the answer list.
        /// </returns>
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
