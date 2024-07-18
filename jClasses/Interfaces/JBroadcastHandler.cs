using System;
using Xamlade.XAMLWorkers;

namespace Xamlade.jClasses;

public interface JBroadcastHandler<T>
    where T : JControl
{
    internal void HandleBroadcast(int mode)
    {
        if((this as JControl).Name==null) return;
        if((this as JControl).Name == "SelectionCanvas") return;
        if(mode == 0) XAMLGenerator.XAMLRatingInit(this as JControl);
        else if (mode == 1) XAMLGenerator.XAMLizeElement(this as JControl);
        else if (mode == 2) ImportXAML.CorrectLoadedjElement(this as JControl);
        else if (mode == 3)
        {
            if( (this as JControl).Name == "MainCanvas") return;
            Broadcast.OnBroadcast -= HandleBroadcast; 
            Broadcast.DisposeElement(this as JControl);
        }
        else if (mode == 4) ImportXAML.CorrectTree(this as JControl);
        else if (mode == 5) 
            if((this as JSelectable). selectionBorder is not null)
                (this as JSelectable).selectionBorder.IsVisible = false;
    }
}