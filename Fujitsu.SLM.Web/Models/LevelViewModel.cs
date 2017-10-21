using Fujitsu.SLM.Constants;
using System.ComponentModel;

namespace Fujitsu.SLM.Web.Models
{
    public abstract class LevelViewModel : BaseViewModel
    {
        private string _editLevel;

        [DisplayName("Level")]
        public string EditLevel
        {
            get { return _editLevel; }
            set
            {
                _editLevel = value ?? string.Empty;
            }
        }

        public bool IsLevelZero => EditLevel == NavigationLevelNames.LevelZero;

        public bool IsLevelOne => EditLevel == NavigationLevelNames.LevelOne;

        public bool IsLevelTwo => EditLevel == NavigationLevelNames.LevelTwo;

        public string ReturnUrl { get; set; }
    }
}