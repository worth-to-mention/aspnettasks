using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingSystem.DataAccess
{
    public enum QuestionType
    {
        Radio,
        Select,
        Text
    }


    public class Question
    {
        public int QuestionID;
        public string Text;
        public QuestionType Type;
        public List<Option> Options;
    }
}
