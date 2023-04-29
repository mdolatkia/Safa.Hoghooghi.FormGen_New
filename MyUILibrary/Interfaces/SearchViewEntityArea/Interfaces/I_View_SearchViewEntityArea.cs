
using CommonDefinitions.UISettings;

using System;
using System.Collections.Generic;
namespace MyUILibrary.EntityArea
{
    public interface I_View_SearchViewEntityArea
    {
        bool HasViewAreaView { get;  }
        bool HasSearchAreaView { get; }
      //  I_View_TemporaryView LastTemporaryView { get; set; }

        void AddSearchAreaView(I_View_SearchEntityArea view);
        void AddViewAreaView(I_View_ViewEntityArea view);
        void RemoveViewAreaView(I_View_ViewEntityArea viewView);
    }

}
