using EPiServer.Shell.Navigation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Framework.Blobs;
using EPiServer.ServiceLocation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using T4;
using OptimizelyGraph.Models.Pages;
using OptimizelyGraph.Models.Media;
using Optimizely.ContentGraph.Cms.NetCore.SearchProviders.Internal;
using System.Collections;
using System.Reflection;
using Microsoft.CodeAnalysis.Operations;
using EPiServer.Core.Internal;
using EPiServer.Security;

namespace OptimizelyGraph.ContentMigration
{

    [MenuProvider]
    public class ContentMigrationMenuProvider : IMenuProvider
    {
        public IEnumerable<MenuItem> GetMenuItems()
        {
            try
            {
                var urlMenuItem1 = new UrlMenuItem("Content Migration", "/global/cms/admin/csp", "/ContentMigrationPage");
                urlMenuItem1.IsAvailable = context => true;
                urlMenuItem1.SortIndex = 500;

                return new List<MenuItem>(1)
                {
                    urlMenuItem1
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

    [Authorize(Roles = "CmsAdmin,WebAdmins,Administrators")]
    [Route("[controller]")]
    public class ContentMigrationPageController : Controller
    {
        private readonly IContentRepository _contentRepository;
        private readonly IBlobFactory _blobFactory;
        private readonly ContentAssetHelper _contentAssetHelper;
        private readonly IMapper _mapper;

        public ContentMigrationPageController(IContentRepository contentRepository, IBlobFactory blobFactory, ContentAssetHelper contentAssetHelper, IMapper mapper)
        {
            _contentRepository = contentRepository;
            _blobFactory = blobFactory;
            _contentAssetHelper = contentAssetHelper;
            _mapper = mapper;
        }

        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                //return View();
                return View("contentmigration/index.cshtml");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [Route("createpage")]
        [HttpGet]
        public IActionResult CreatePage()
        {
            try
            {
                var parent = _contentRepository.Get<StandardPage>(new ContentReference(10));
                ArticlePage myPage = _contentRepository.GetDefault<ArticlePage>(parent.ContentLink);

                myPage.Name = "NewPage";
                myPage.MetaTitle = "sdfsdf";
                _contentRepository.Save(myPage, SaveAction.Publish);

                return Index();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private byte[] FindBlob(string id)
        {
            try
            {
                var folderItem = new DirectoryInfo(@"C:\Migration\Habitat.1.4.0.682\package\blob\master");

                if (folderItem != null)
                {
                    var file = folderItem.GetFiles().FirstOrDefault(x => x.Name.Equals(id.Replace("{", "").Replace("}", ""), StringComparison.OrdinalIgnoreCase));
                    if (file != null)
                    {
                        return System.IO.File.ReadAllBytes(file.FullName);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private ContentReference CreateImage(ContentReference reference, FieldDetails<ImageField> image)
        {
            try
            {
                byte[] data = FindBlob(image.Value.BlobId);

                var imageFile = _contentRepository.GetDefault<GenericMedia>(_contentAssetHelper.GetOrCreateAssetFolder(reference).ContentLink);

                imageFile.Name = image.Value.Name;

                var blob = _blobFactory.CreateBlob(imageFile.BinaryDataContainer, "." + image.Value.Extension);
                using (var s = blob.OpenWrite())
                {
                    var w = new StreamWriter(s);
                    w.BaseStream.Write(data, 0, data.Length);
                    w.Flush();
                }
                imageFile.BinaryData = blob;
                return _contentRepository.Save(imageFile, SaveAction.Publish);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [Route("ExecuteImport")]
        [HttpGet]
        public IActionResult ExecuteImport()
        {
            try
            {
                ContentTyped mapped;
                using (StreamReader r = new StreamReader(Path.Combine(Directory.GetCurrentDirectory() + @"\wwwroot\Input\ContentTyped.json")))
                {
                    string json = r.ReadToEnd();
                    mapped = JsonConvert.DeserializeObject<ContentTyped>(json);
                }
                GetPageTypeByJSONFormat(mapped);
                return Index();
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        private void GetPageTypeByJSONFormat(ContentTyped obj)
        {
            try
            {
                string nameSpaceForPageType = "OptimizelyGraph.Models.Pages";
                PropertyInfo[] properties = typeof(ContentTyped).GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        Type itemType = property.PropertyType.GetGenericArguments()[0];
                        string className = itemType.Name;
                        IList list = (IList)property.GetValue(obj);
                        if (list != null)
                        {
                            string OptimizelyCMSPageName = className + "Page";
                            nameSpaceForPageType = "OptimizelyGraph.Models.Pages" + "." + OptimizelyCMSPageName;
                            Assembly assembly = Assembly.GetExecutingAssembly();
                            Type type = assembly.GetType(nameSpaceForPageType);
                            if (type != null){
                                foreach (var item in list) {
                                    MethodInfo[] methods = _contentRepository.GetType().GetMethods().Where(m => m.Name == "GetDefault").ToArray();
                                    MethodInfo method = methods.FirstOrDefault(m => m.GetGenericArguments().Length == 1);
                                    if (method != null){
                                        MethodInfo genericMethod = method.MakeGenericMethod(type);
                                        var parent = _contentRepository.Get<StandardPage>(new ContentReference(10));
                                        object myPage = genericMethod.Invoke(_contentRepository, new object[] { parent.ContentLink });
                                        dynamic myPageDynamic = (dynamic)myPage;

                                        var employeeModel = _mapper.Map(item, myPage);
                                        _contentRepository.Save((IContent)myPage, SaveAction.Publish, AccessLevel.Create);

                                        dynamic itemDynamic = (dynamic)item;
                                        var imageRef = CreateImage(myPageDynamic.ContentLink, itemDynamic.Image);
                                        myPageDynamic.PageImage = imageRef;
                                        _contentRepository.Save((IContent)myPageDynamic, SaveAction.Publish, AccessLevel.Create);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                Console.Write("Automapper property mapping error");

            }
        }

        private void Import()
        {
            try
            {
                //https://www.newtonsoft.com/json/help/html/CreatingLINQtoJSON.htm

                var mappingModel = GetModel();
                var content = GetContent();

                foreach (var item in content)
                {
                    var mappingDef = mappingModel.TemplateMappingDefinition.FirstOrDefault(x => x.SourceTemplateId.Equals(item.SourceTemplate, StringComparison.InvariantCultureIgnoreCase));

                    if (mappingDef == null)
                    {
                        continue;
                    }

                    JObject postData =
                            new JObject(
                                new JProperty("name", item.SourceName),
                                new JProperty("language", new JObject(new JProperty("name", mappingDef.DestinationLanguage))),
                                new JProperty("contentType", new JArray(new JValue(mappingDef.DestinationContentType))),
                                new JProperty("parentLink", new JObject(new JProperty("id", mappingDef.DestinationParentId))),
                                new JProperty("status", "CheckedOut"));
                    foreach (var fieldMap in mappingDef.FieldMappings)
                    {
                        var contentRef = item.SourceFields.Where(x => x.SourceName.Equals(fieldMap.SourceField, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                        if (contentRef != null)
                        {
                            postData.Add(fieldMap.DestinationField, new JObject(new JProperty("value", contentRef.SourceValue)));
                        }
                    }

                }



            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private Root GetModel()
        {
            try
            {
                Root mappingModel;
                using (StreamReader r = new StreamReader(Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\Input\\Mapping.json")))
                {
                    string json = r.ReadToEnd();
                    mappingModel = JsonConvert.DeserializeObject<Root>(json);
                }
                return mappingModel;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private List<OptimizelyContentModel> GetContent()
        {
            try
            {
                List<OptimizelyContentModel> content;
                using (StreamReader r = new StreamReader(Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\Input\\Content.json")))
                {
                    string json = r.ReadToEnd();
                    content = JsonConvert.DeserializeObject<List<OptimizelyContentModel>>(json);
                }
                return content;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
