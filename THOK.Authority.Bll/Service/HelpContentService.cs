using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using THOK.Authority.Bll.Interfaces;
using THOK.Authority.Bll.Models;
using THOK.Authority.Dal.Interfaces;
using THOK.Authority.DbModel;

namespace THOK.Authority.Bll.Service
{
    public class HelpContentService : ServiceBase<AUTH_HELP_CONTENT>, IHelpContentService
    {
        [Dependency]
        public ISystemRepository SystemRepository { get; set; }
        [Dependency]
        public IModuleRepository ModuleRepository { get; set; }
        [Dependency]
        public IHelpContentRepository HelpContentRepository { get; set; }

        [Dependency]
        public IExceptionalLogService ExceptionalLogService { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }


        public bool Add(AUTH_HELP_CONTENT helpContent, out string strResult)
        {
                strResult = string.Empty;
                bool result = false;
                var help = new AUTH_HELP_CONTENT();
                if (helpContent != null)
                {
                    try
                    {
                        help.ID = HelpContentRepository.GetNewID("AUTH_HELP_CONTENT", "ID");
                        help.CONTENT_CODE = helpContent.CONTENT_CODE;
                        help.CONTENT_NAME = helpContent.CONTENT_NAME;
                        help.CONTENT_PATH = helpContent.CONTENT_PATH;
                        help.NODE_TYPE = helpContent.NODE_TYPE;
                        help.FATHER_NODE_ID = helpContent.FATHER_NODE_ID != null ? helpContent.FATHER_NODE_ID : help.ID;
                        help.MODULE_ID = helpContent.MODULE_ID;
                        help.NODE_ORDER = helpContent.NODE_ORDER;
                        help.IS_ACTIVE = helpContent.IS_ACTIVE;
                        help.UPDATE_TIME = DateTime.Now;

                        HelpContentRepository.Add(help);
                        HelpContentRepository.SaveChanges();
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        strResult = "原因：" + ex.Message;
                       
                    }
                }
        
            return result;
        }

        public bool Add(string ID, string ContentCode, string ContentName, string ContentPath,string NODETYPE, string FatherNodeID, string ModuleID, int NodeOrder, string IsActive, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var help = new AUTH_HELP_CONTENT();
            try
            {
                help.ID = HelpContentRepository.GetNewID("AUTH_HELP_CONTENT", "ID");
                help.CONTENT_CODE = ContentCode;
                help.CONTENT_NAME = ContentName;
                help.CONTENT_PATH = ContentPath;
                help.NODE_TYPE = NODETYPE;
                help.FATHER_NODE_ID = string.IsNullOrEmpty(FatherNodeID) ? help.ID : FatherNodeID;
                help.MODULE_ID = ModuleID;
                help.NODE_ORDER = NodeOrder;
                help.IS_ACTIVE = IsActive;
                help.UPDATE_TIME = DateTime.Now;

                HelpContentRepository.Add(help);
                HelpContentRepository.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                HelpContentRepository.Detach(help);
                strResult = "原因：" + ex.Message;
                ExceptionalLogService.Add(this.GetType().ToString(), "Add", ex);
                result = false;
            }
           
            return result;
        }
           

        public string WhatType(string nodeType)
        {
            string typeStr = "";
            switch (nodeType)
            {
                case "1":
                    typeStr = "第一节点";
                    break;
                case "2":
                    typeStr = "中间节点";
                    break;
                case "3":
                    typeStr = "末端节点";
                    break;
            }
            return typeStr;
        }
        public object GetDetails(int page, int rows, string QueryString, string Value)
        {
            string ContentName = "";
            string ContentCode = "";
            if (QueryString == "ContentName")
            {
                ContentName = Value;
            }
            else
            {
                ContentCode = Value;
            }
            IQueryable<AUTH_HELP_CONTENT> HelpContentQuery = HelpContentRepository.GetQueryable();
            var HelpContent = HelpContentQuery.Where(c => c.CONTENT_NAME.Contains(ContentName) && c.CONTENT_CODE.Contains(ContentCode))
                .OrderBy(c => c.CONTENT_CODE)
                .Select(c => c);
            if (!ContentName.Equals(string.Empty))
            {
                HelpContent = HelpContent.Where(p => p.CONTENT_NAME == ContentName);
            }
            int total = HelpContent.Count();
            HelpContent = HelpContent.Skip((page - 1) * rows).Take(rows);

            var temp = HelpContent.ToArray().Select(c => new
            {
                ID = c.ID,
                ContentCode = c.CONTENT_CODE,
                ContentName = c.CONTENT_NAME,
                FatherNode = c.ID ==c.FATHER_NODE_ID ? "":  c.FATHER_NODE.CONTENT_CODE + " " + c.FATHER_NODE.CONTENT_NAME,
                NodeType = WhatType(c.NODE_TYPE),
                NodeOrder = c.NODE_ORDER,
                IsActive = c.IS_ACTIVE == "1" ? "可用" : "不可用"
            });
            return new { total, rows = temp.ToArray() };
        }

        public bool Save(string ID, string ContentCode, string ContentName, string ContentPath, string FatherNodeID, string ModuleID, int NodeOrder, string IsActive, out string strResult)
        {
            strResult = string.Empty;
            try
            {
                //Guid new_ID = new Guid(ID);
                var help = HelpContentRepository.GetQueryable()
                    .FirstOrDefault(i => i.ID == ID);
                help.CONTENT_CODE = ContentCode;
                help.CONTENT_NAME = ContentName;
                help.CONTENT_PATH = ContentPath;
                help.FATHER_NODE_ID = FatherNodeID;
                help.MODULE_ID = ModuleID;
                help.NODE_ORDER = NodeOrder;
                help.IS_ACTIVE = IsActive;
               int result= HelpContentRepository.SaveChanges();
               if (result == -1)
               {
                   return false;
               }
               else return true;
            }
            catch (Exception ex)
            {
                strResult = "原因：" + ex.Message;
                return false;
            }
            //return true;
        }

        public object GetDetails2(int page, int rows, string ContentCode, string ContentName, string NodeType, string FatherNodeID, string ModuleID, string IsActive)
        {
            IQueryable<AUTH_HELP_CONTENT> HelpContentQuery = HelpContentRepository.GetQueryable();
            var HelpContent = HelpContentQuery.Where(c => c.CONTENT_CODE.Contains(ContentCode) &&
                                                          c.CONTENT_NAME.Contains(ContentName) &&
                                                          c.IS_ACTIVE.Contains(IsActive) &&
                                                          c.NODE_TYPE.Contains(NodeType));
            if (!FatherNodeID.Equals(string.Empty) && FatherNodeID != null)
            {
               // Guid Father_NodeID = new Guid(FatherNodeID);

                HelpContent = HelpContent.Where(h => h.FATHER_NODE_ID == FatherNodeID);
            }
            if (!ModuleID.Equals(string.Empty) && ModuleID != null)
            {
                //Guid Module_ID = new Guid(ModuleID);
                HelpContent = HelpContent.Where(h => h.MODULE_ID == ModuleID);
            }
            HelpContent = HelpContent.OrderBy(h => h.CONTENT_CODE);
            int total = HelpContent.Count();
            HelpContent = HelpContent.Skip((page - 1) * rows).Take(rows);

            var temp = HelpContent.ToArray().Select(c => new
            {
               ID=c.ID,
               ContentCode= c.CONTENT_CODE,
               ContentName= c.CONTENT_NAME,
               ContentPath= c.CONTENT_PATH,
               FatherNodeName =c.ID ==c.FATHER_NODE_ID? "": c.FATHER_NODE.CONTENT_NAME,
               ModuleID= c.MODULE_ID,
               ModuleName = c.AUTH_MODULE.MODULE_NAME,
               FatherNodeID= c.FATHER_NODE_ID,
               NodeType= WhatType(c.NODE_TYPE),
               NodeOrder= c.NODE_TYPE,
               IsActive = c.IS_ACTIVE == "1" ? "可用" : "不可用",
               UpdateTime=c.UPDATE_TIME.ToString("yyyy-MM-dd HH:mm:ss")
            });
            return new { total, rows = temp.ToArray() };
        }

        public bool Delete(string ID)
        {
            //Guid new_ID = new Guid(ID);
            var help = HelpContentRepository.GetQueryable()
                .FirstOrDefault(i => i.ID == ID);
            if (ID != null)
            {
                HelpContentRepository.Delete(help);
                HelpContentRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }
        public object GetHelpContentTree(string sysId)
        {
            IQueryable<THOK.Authority.DbModel.AUTH_SYSTEM> querySystem = SystemRepository.GetQueryable();
            IQueryable<THOK.Authority.DbModel.AUTH_MODULE> queryModule = ModuleRepository.GetQueryable();
            IQueryable<THOK.Authority.DbModel.AUTH_HELP_CONTENT> queryHelpContent = HelpContentRepository.GetQueryable();
            var systems = querySystem.AsEnumerable();
            if (sysId != null && sysId != string.Empty)
            {
                //Guid gsystemid = new Guid(sysId);
                systems = querySystem.Where(i => i.SYSTEM_ID == sysId)
                                     .Select(i => i);
            }

            HashSet<Tree> systemTreeSet = new HashSet<Tree>();
            foreach (var system in systems)
            {
                Tree systemTree = new Tree();
                systemTree.id = int.Parse(system.SYSTEM_ID).ToString();
                systemTree.text = system.SYSTEM_NAME;
                var helpContent = queryHelpContent.Where(m => m.AUTH_MODULE.SYSTEM_SYSTEM_ID == system.SYSTEM_ID && m.ID == m.FATHER_NODE_ID)
                                         .OrderBy(m => m.NODE_ORDER)
                                         .Select(m => m);
                var systemAttribute = new
                {
                    AttributeId =int.Parse(system.SYSTEM_ID).ToString(),
                    AttributeTxt =system.SYSTEM_NAME
                };
                systemTree.attributes = systemAttribute;
                HashSet<Tree> contentTreeSet = new HashSet<Tree>();
                foreach (var item in helpContent)
                {
                    Tree helpContentTree = new Tree();
                    helpContentTree.id = item.CONTENT_CODE;
                    helpContentTree.text = item.CONTENT_CODE + item.CONTENT_NAME;
                    var helpAttribute = new
                    {
                        AttributeId = item.ID,
                        AttributeTxt = item.CONTENT_TEXT
                    };
                    helpContentTree.attributes = helpAttribute;
                    contentTreeSet.Add(helpContentTree);
                    GetChildTree(helpContentTree, item);
                    contentTreeSet.Add(helpContentTree);
                }
                systemTree.children = contentTreeSet.ToArray();
                systemTreeSet.Add(systemTree);
            }
            return systemTreeSet.ToArray();
        }
        private void GetChildTree(Tree helpContentTree, AUTH_HELP_CONTENT helpContent)
        {
            HashSet<Tree> childContentSet = new HashSet<Tree>();
            var helpContents = from m in helpContent.AUTH_HELP_CONTENTS
                          orderby m.NODE_ORDER
                               where m.FATHER_NODE_ID == helpContent.FATHER_NODE_ID
                          select m;
            foreach (var item in helpContents)
            {
                if (item.ID != helpContent.ID)
                {
                    Tree childContentTree = new Tree();
                    childContentTree.id = item.CONTENT_CODE;
                    childContentTree.text = item.CONTENT_CODE + item.CONTENT_NAME;
                    var childAttribute = new
                    {
                        AttributeId = item.ID,
                        AttributeTxt = item.CONTENT_TEXT
                    };
                    childContentTree.attributes = childAttribute;
                    childContentSet.Add(childContentTree);
                }
            }
            helpContentTree.children = childContentSet.ToArray();
        }
        public bool EditSave(string helpId, string contentText, out string strResult)
        {
            strResult = string.Empty;
            try
            {
                //Guid new_ID = new Guid(helpId);
                var help = HelpContentRepository.GetQueryable()
                    .FirstOrDefault(i => i.ID == helpId);
                help.CONTENT_TEXT = contentText;
                HelpContentRepository.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                strResult = "原因：" + ex.Message;
                return false;
            }
        }

        public object GetContentTxt(string helpId)
        {
            string content_text = "";
            //Guid new_ID = new Guid(helpId);
            string new_ID = helpId.PadLeft(3,'0');
            var help = HelpContentRepository.GetQueryable().FirstOrDefault(i => i.ID == helpId);
            if (help == null)
            {
                var system = SystemRepository.GetQueryable().FirstOrDefault(i => i.SYSTEM_ID == new_ID);
                var Systemhelp = HelpContentRepository.GetQueryable().Where(i => i.AUTH_MODULE.SYSTEM_SYSTEM_ID == new_ID).OrderBy(h => h.CONTENT_CODE);
                foreach (var text in Systemhelp)
                {
                    content_text = content_text + text.CONTENT_TEXT;
                }
                var helper = HelpContentRepository.GetQueryable().FirstOrDefault(h => h.AUTH_MODULE.SYSTEM_SYSTEM_ID == new_ID);
                if (helper == null)
                    helper = new AUTH_HELP_CONTENT();
                helper.CONTENT_TEXT = content_text;
                return new { helper.CONTENT_TEXT };
            }
            else
            {
                if (help.NODE_TYPE == "1")
                {
                    var helpChild = HelpContentRepository.GetQueryable().Where(i => i.FATHER_NODE_ID == help.FATHER_NODE_ID).OrderBy(h => h.CONTENT_CODE);
                    foreach (var text in helpChild)
                    {
                        content_text = content_text + text.CONTENT_TEXT;
                    }
                    help.CONTENT_TEXT = content_text;
                    return new { help.CONTENT_TEXT };
                }
                else
                {
                    return new { help.CONTENT_TEXT };
                }
            }
        }

        public object Help(string helpId)
        {
            //Guid new_ID = new Guid(helpId);
            var help = HelpContentRepository.GetQueryable().FirstOrDefault(i => i.MODULE_ID == helpId);
            if (help == null)
            {
                help = new AUTH_HELP_CONTENT();
            }
            return new { help.CONTENT_TEXT };
        }


        public object GetSingleContentTxt(string helpId)
        {
            //Guid newId = new Guid(helpId);
            string newId = helpId.PadLeft(3,'0');
            var help = HelpContentRepository.GetQueryable().FirstOrDefault(i => i.ID == helpId);
            if (help == null)
            {
                var system = SystemRepository.GetQueryable().FirstOrDefault(i => i.SYSTEM_ID == newId);
                if (system != null)
                {
                    help = HelpContentRepository.GetQueryable().FirstOrDefault(i => i.AUTH_MODULE.SYSTEM_SYSTEM_ID == newId);
                }
            }
            if (help == null)
            {
                help = new AUTH_HELP_CONTENT();
            }
            return new { help.CONTENT_TEXT };
            
        }
    }
}
