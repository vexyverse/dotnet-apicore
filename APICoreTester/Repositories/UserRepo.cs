﻿using System;
using System.Collections.Generic;
using System.Linq;
using APICore;
using APICore.Models;
using static APICore.Configurations;
using APICore.Security.Tokenizers;
using Dapper;

namespace APICoreTester.Repositories
{
    public class UserRepo
    {
        private readonly Queries _query;

        public UserRepo()
        {
            _query = new Queries("Users");
        }

        public Response<User> Login(User model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Surname))
                {                    
                    throw new Exception("Empty surname !");
                }

                JwtToken.Sign(model);
                
                return new Response<User> { Status = true, Data = model, Message = GetMessage(MessageTypes.Login, true) };
            }
            catch (Exception ex)
            {
                return new Response<User> { Status = false, Message = ex.Message};
            }
        }

        public Response<List<User>> GetAll()
        {
            try
            {
                using var con = CreateDatabaseConnection();
                var res = con.Query<User>(_query.Select()).ToList();
                return new Response<List<User>> { Status = true, Data = res, Message = GetMessage(MessageTypes.GetAll, true)};
            }
            catch (Exception ex)
            {
                return new Response<List<User>> { Status = false, Message = ex.Message};
            }
        }

        public Response<User> Get(User model)
        {
            try
            {
                using var con = CreateDatabaseConnection();
                var res = con.QuerySingle<User>(_query.Select(options: Queries.WhereQuery("Id", model.Id)));
                return new Response<User> { Status = true, Data = res, Message = GetMessage(MessageTypes.Get, true)};
            }
            catch (Exception ex)
            {
                return new Response<User> { Status = false, Message = ex.Message};
            }
        }

        public Response<dynamic> Add(User model)
        {
            try
            {
                using var con = CreateDatabaseConnection();
                var res = con.Execute(_query.Insert( new[]
                {
                    "Title",
                    "Description",
                    "ImageUrl"
                }), model);
                return res > 0 ? 
                    new Response<dynamic> { Status = true, Message = GetMessage(MessageTypes.Add, true) } :
                    new Response<dynamic> { Status = false, Message = GetMessage(MessageTypes.Add) };
            }
            catch (Exception ex)
            {
                return new Response<dynamic> { Status = false, Message = ex.Message };
            }
        }

        public Response<dynamic> Update(User model)
        {
            try
            {
                using var con = CreateDatabaseConnection();
                var res = con.Execute(_query.Update( new []
                {
                    "Title", 
                    "Description", 
                    "ImageUrl"
                }, Queries.WhereQuery("Id", model.Id)), model);
                return res > 0 ? 
                    new Response<dynamic> { Status = true, Data = res, Message = GetMessage(MessageTypes.Update, true) } :
                    new Response<dynamic> { Status = false, Data = res, Message = GetMessage(MessageTypes.Update) };
            }
            catch (Exception ex)
            {
                return new Response<dynamic> { Status = false, Message = ex.Message};
            }
        }

        public Response<dynamic> Delete(User model)
        {
            try
            {
                using var con = CreateDatabaseConnection();
                var res = con.Execute(_query.Delete( Queries.WhereQuery("Id", model.Id)));
                return res > 0 ? 
                    new Response<dynamic> { Status = true, Data = res, Message = GetMessage(MessageTypes.Delete, true) } :
                    new Response<dynamic> { Status = false, Data = res, Message = GetMessage(MessageTypes.Delete) };
            }
            catch (Exception ex)
            {
                return new Response<dynamic> { Status = false, Message = ex.Message};
            }
        }
    }
}