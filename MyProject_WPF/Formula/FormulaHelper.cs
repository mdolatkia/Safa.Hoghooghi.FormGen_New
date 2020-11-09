using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelEntites;
using MyFormulaFunctionStateFunctionLibrary;
using ProxyLibrary;

namespace MyProject_WPF
{
    public class FormulaHelper
    {
        internal static FormulaItemDTO ToFormulaItem(MyPropertyInfo calledProperty)
        {
            var newItem = new FormulaItemDTO();
            newItem.ItemType = GetFormulaItemType(calledProperty.PropertyType);
            if (newItem.ItemType == FormuaItemType.None)
                return null;
            newItem.ItemID = calledProperty.ID;
            newItem.ItemTitle = calledProperty.Name;
            newItem.RelationshipIDTail = calledProperty.RelationshipTail;
            newItem.RelationshipNameTail = calledProperty.RelationshipPropertyTail;
            return newItem;
        }
        internal static FormuaItemType GetFormulaItemType(PropertyType propertyType)
        {
            if (propertyType == PropertyType.Column)
                return FormuaItemType.Column;
            else if (propertyType == PropertyType.Relationship)
                return FormuaItemType.Relationship;
            else if (propertyType == PropertyType.FormulaParameter)
                return FormuaItemType.FormulaParameter;
            //else if (propertyType == PropertyType.DBFormula)
            //    return FormuaItemType.DatabaseFunction;
            else if (propertyType == PropertyType.Code)
                return FormuaItemType.Code;
            else if (propertyType == PropertyType.State)
                return FormuaItemType.State;
            //else if (propertyType == PropertyType.Helper)
            //    return FormuaItemType.he;
            return FormuaItemType.None;
        }
    }
}
