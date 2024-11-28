using System;
using Xamlade.XAMLWorkers;

namespace Xamlade.jClasses;

public interface JBroadcastHandler<T>
    where T : JControl
{
    internal void HandleBroadcast(int mode)
    {
        switch ((this as JControl).Name)
        {
            case null:
            case "SelectionCanvas":
                return;
        }
        switch (mode)
        {
            case 0:
                XAMLGenerator.XAMLRatingInit(this as JControl);
                break;
            case 1:
                XAMLGenerator.XAMLizeElement(this as JControl);
                break;
            case 2:
                ImportXAML.CorrectLoadedjElement(this as JControl);
                break;
            case 3 when (this as JControl).Name == "MainCanvas":
                return;
            case 3:
                Broadcast.OnBroadcast -= HandleBroadcast; 
                Broadcast.DisposeElement(this as JControl);
                break;
            case 4:
                ImportXAML.CorrectTree(this as JControl);
                break;
            case 5:
            {
                if((this as JControl).Beholder.selectionBorder is not null )
                    (this as JControl).Beholder.selectionBorder.IsVisible = false;
                break;
            }
        }
    }
}