using DataAccess;
using ProxyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyModelManager
{
    public class BizOrganizationType
    {
        public List<OrganizationTypeDTO> GetAllOrganizationTypes()
        {
            var context = new MyIdeaEntities();
            var organizationTypes = context.OrganizationType;
            List<OrganizationTypeDTO> result = new List<OrganizationTypeDTO>();
            foreach (var item in organizationTypes)
            {
                OrganizationTypeDTO OrganizationTypeDto = ToOrganizationTypeDTO(item, false);
                result.Add(OrganizationTypeDto);
            }
            return result;
        }
        public OrganizationTypeDTO GetOrganizationType(int ID, bool withDetails)
        {
            var context = new MyIdeaEntities();
            var dborganizationType = context.OrganizationType.First(x => x.ID == ID);
            return ToOrganizationTypeDTO(dborganizationType, withDetails);
        }
        private OrganizationTypeDTO ToOrganizationTypeDTO(OrganizationType item, bool withDetails)
        {
            OrganizationTypeDTO result = new OrganizationTypeDTO();
            result.ID = item.ID;
            result.Name = item.Name;
            result.ExternalKey = item.ExternalKey;
            if (withDetails)
            {
                foreach (var child in item.OrganizationType_RoleType)
                {
                    OrganizationTypeRoleTypeDTO cItem = new OrganizationTypeRoleTypeDTO();
                    cItem.ID = child.ID;
                    cItem.Name = child.OrganizationType.Name + "-" + child.RoleType.Name;
                    cItem.IsAdmin = child.IsAdmin == true;
                    cItem.RoleTypeID = child.RoleTypeID;
                    cItem.ExternalKey = child.ExternalKey;
                    cItem.OrganizationTypeID = child.OrganizationTypeID;
                    result.OrganizationTypeRoleTypes.Add(cItem);
                }
            }
            return result;
        }

        public int SaveOrganizationType(OrganizationTypeDTO OrganizationTypeDto)
        {
            using (var context = new MyIdeaEntities())
            {

                OrganizationType dbOrganizationType = null;
                if (OrganizationTypeDto.ID == 0)
                {
                    dbOrganizationType = new OrganizationType();
                    dbOrganizationType.SecuritySubject=new SecuritySubject();
                    dbOrganizationType.SecuritySubject.Type = (int)SecuritySubjectType.OrganizationType;
                }
                else
                    dbOrganizationType = context.OrganizationType.First(x => x.ID == OrganizationTypeDto.ID);

                dbOrganizationType.Name = OrganizationTypeDto.Name;
                dbOrganizationType.ExternalKey = OrganizationTypeDto.ExternalKey;
                //چیزی حذف نمیشود
                foreach (var orgTypeRoleType in OrganizationTypeDto.OrganizationTypeRoleTypes)
                {
                    var dborgTypeRoleType = dbOrganizationType.OrganizationType_RoleType.FirstOrDefault(x => x.ID == orgTypeRoleType.ID);
                    if (dborgTypeRoleType == null)
                    {
                        dborgTypeRoleType = new  OrganizationType_RoleType();
                        dbOrganizationType.OrganizationType_RoleType.Add(dborgTypeRoleType);
                        dborgTypeRoleType.SecuritySubject=new SecuritySubject();
                        dborgTypeRoleType.SecuritySubject.Type = (int)SecuritySubjectType.OrganizationTypeRoleType;
                    }
                    dborgTypeRoleType.IsAdmin = orgTypeRoleType.IsAdmin;
                    dborgTypeRoleType.ExternalKey = orgTypeRoleType.ExternalKey;
                    dborgTypeRoleType.RoleTypeID = orgTypeRoleType.RoleTypeID;
                   
                }

                if (dbOrganizationType.ID == 0)
                    context.OrganizationType.Add(dbOrganizationType);
                context.SaveChanges();
                return dbOrganizationType.ID;
            }
        }



    }




}
