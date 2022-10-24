using ModelEntites;
using MyModelManager;

using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRelationshipDataManager
{
    public class RelationshipTailDataManager
    {
        BizRelationship bizRelationship = new BizRelationship();
        BizEntityRelationshipTail bizRelationshipTail = new BizEntityRelationshipTail();
        // SecurityHelper securityHelper = new SecurityHelper();
        //public EntityRelationshipTailDTO GetRelationshipTail(int id)
        //{
        //    return bizRelationshipTail.GetEntityRelationshipTail(id);
        //}
        //مهم اینه که داده مبدا از ویو نباشد
        public DP_SearchRepositoryMain GetTargetSearchItemFromRelationshipTail(DP_BaseData firstDataItem, EntityRelationshipTailDTO relationshipTail)
        {

            var linkedRelationshipTails = GetLinkedRelationshipTails(relationshipTail.ReverseRelationshipTail);

            DP_SearchRepositoryMain result = new DP_SearchRepositoryMain(firstDataItem.TargetEntityID);
            foreach (var column in firstDataItem.KeyProperties)
                result.Phrases.Add(new SearchProperty() { ColumnID = column.ColumnID, Value = column.Value });


            GetSourceSideSearchItemFromRelationshipTail(linkedRelationshipTails.First, result);
            return result;
        }

        private void GetSourceSideSearchItemFromRelationshipTail(LinkedListNode<EntityRelationshipTailDTO> linkedListNode, LogicPhraseDTO logicPhrase )
        {

            if (linkedListNode != null)
            {
                DP_SearchRepositoryRelationship rel = new DP_SearchRepositoryRelationship();
                rel.SourceRelationship = linkedListNode.Value.Relationship;
                logicPhrase.Phrases.Add(rel);

                GetSourceSideSearchItemFromRelationshipTail(linkedListNode.Next, logicPhrase);
            }
        }



        private LinkedList<EntityRelationshipTailDTO> GetLinkedRelationshipTails(EntityRelationshipTailDTO firstRelationshipTail)
        {
            LinkedList<EntityRelationshipTailDTO> result = new LinkedList<EntityRelationshipTailDTO>();

            List<EntityRelationshipTailDTO> list = GetListOfRelationshipTails(firstRelationshipTail);
            foreach (var item in list)
            {
                result.AddLast(item);
            }
            return result;
        }

        private List<EntityRelationshipTailDTO> GetListOfRelationshipTails(EntityRelationshipTailDTO relationshipTail, List<EntityRelationshipTailDTO> items = null)
        {
            if (items == null)
                items = new List<EntityRelationshipTailDTO>();
            if (relationshipTail != null)
            {
                items.Add(relationshipTail);
                return GetListOfRelationshipTails(relationshipTail.ChildTail, items);
            }
            else
                return items;
        }

        //public EntityRelationshipTailDTO JoinRelationshipTail(EntityRelationshipTailDTO relationshipTail1, EntityRelationshipTailDTO relationshipTail2)
        //{
        //    return bizRelationshipTail.JoinRelationshipTail(relationshipTail1, relationshipTail2);
        //}
    }
    public enum RelationshipTailSreachType
    {
        SourceBasedOnTargetKeyColumn,
        TargetBasedOnSourceKeyColumn
    }
}
