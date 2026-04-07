#if TOOLS
using Godot;
using System;
namespace GameplayToolkit.ToolsAndEdit
{
    [GlobalClass, Tool, Icon(IconPaths.THUMBTACK)]
    public partial class SubTask : Resource
    {
        [Signal] public delegate void SubTitleUpdatedEventHandler();
        [Signal] public delegate void IsCompletedUpdatedEventHandler();
        
        private string _subTitle;
        [Export(PropertyHint.MultilineText)] public string SubTitle
        {
            get =>_subTitle;
            set
            {
                if(_subTitle != value)
                {
                    _subTitle = value;
                    EmitSignal(SignalName.SubTitleUpdated);
                }
            }
        }
        private bool _isCompleted;
        [Export] public bool IsCompleted
        {
            get =>_isCompleted;
            set
            {
                if(_isCompleted != value)
                {
                    _isCompleted = value;
                    EmitSignal(SignalName.IsCompletedUpdated);
                }
            }
        }
    }
}
#endif