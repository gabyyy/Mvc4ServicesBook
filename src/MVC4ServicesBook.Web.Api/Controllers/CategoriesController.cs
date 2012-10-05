﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MVC4ServicesBook.Data;
using MVC4ServicesBook.Web.Api.Models;
using MVC4ServicesBook.Web.Common;

namespace MVC4ServicesBook.Web.Api.Controllers
{
    [LoggingNHibernateSessions]
    public class CategoriesController : ApiController
    {
        private readonly ICommonRepository _commonRepository;

        public CategoriesController(ICommonRepository commonRepository)
        {
            _commonRepository = commonRepository;
        }

        public IEnumerable<Category> Get()
        {
            return _commonRepository
                .GetAll<Data.Model.Category>()
                .Select(CreateCategoryResponse)
                .ToList();
        }

        public Category Get(long id)
        {
            var category = _commonRepository.Get<Data.Model.Category>(id);
            if(category == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return CreateCategoryResponse(category);
        }

        private Category CreateCategoryResponse(Data.Model.Category modelCategory)
        {
            return new Category
                       {
                           CategoryId = modelCategory.CategoryId,
                           Description = modelCategory.Description,
                           Name = modelCategory.Name,
                           Links = new List<Link>
                                       {
                                           new Link
                                               {
                                                   Title = "self",
                                                   Rel = "self",
                                                   Href = "/api/categories/" + modelCategory.CategoryId
                                               }
                                       }
                       };
        }

        public HttpResponseMessage Post(HttpRequestMessage request, Category category)
        {
            var modelCategory = new Data.Model.Category
                                    {
                                        Description = category.Description,
                                        Name = category.Name
                                    };

            _commonRepository.Save(modelCategory);

            var newCategory = CreateCategoryResponse(modelCategory);
            var response = request.CreateResponse(HttpStatusCode.Created, newCategory);
            response.Headers.Add("Location", "/api/categories/" + newCategory.CategoryId);

            return response;
        }

        public HttpResponseMessage Delete()
        {
            var categories = _commonRepository.GetAll<Data.Model.Category>().ToList();
            foreach (var category in categories)
            {
                _commonRepository.Delete(category);
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }      
        
        public HttpResponseMessage Delete(long id)
        {
            var category = _commonRepository.Get<Data.Model.Category>(id);
            if (category != null)
            {
                _commonRepository.Delete(category);
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        public Category Put(long id, Category category)
        {
            var modelCateogry = _commonRepository.Get<Data.Model.Category>(id);
            if (modelCateogry == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            modelCateogry.Name = category.Name;
            modelCateogry.Description = category.Description;
            
            _commonRepository.Save(modelCateogry);

            return CreateCategoryResponse(modelCateogry);
        }
    }
}
