using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Fujitsu.SLM.Data.Interfaces;
using Moq;

namespace Fujitsu.SLM.UnitTesting
{
    [ExcludeFromCodeCoverage]
    public class MockRepositoryHelper
    {
        public static Mock<IRepository<T>> Create<T>(List<T> itemList) where T : class
        {
            return Create(itemList, null, null);
        }

        public static Mock<IRepository<T>> Create<T>(List<T> itemList, Func<T, object, bool> idFunc) where T : class
        {
            return Create(itemList, idFunc, null);
        }

        public static Mock<IRepository<T>> Create<T>(List<T> itemList, Func<T, T, bool> equalsFunc) where T : class
        {
            return Create(itemList, null, equalsFunc);
        }

        public static Mock<IRepository<T>> Create<T>(List<T> itemList, Func<T, object, bool> idFunc, Func<T, T, bool> equalsFunc) where T : class
        {
            var response = new Mock<IRepository<T>>();
            response.Setup(moq => moq.Insert(It.IsAny<T>())).Callback((T item) => itemList.Add(item));
            response.Setup(moq => moq.AddRange(It.IsAny<List<T>>())).Callback<List<T>>(itemList.AddRange);
            response.Setup(moq => moq.Delete(It.IsAny<T>())).Callback((T item) => itemList.Remove(item));
            response.Setup(moq => moq.Delete(It.IsAny<IEnumerable<T>>()))
                            .Callback(delegate(IEnumerable<T> items)
                            {
                                foreach (var i in items)
                                {
                                    itemList.Remove(i);
                                }
                            });

            response.Setup(moq => moq.Find(It.IsAny<Expression<Func<T, bool>>>())).Returns((Expression<Func<T, bool>> predicate) => itemList.Where(predicate.Compile()));
            response.Setup(moq => moq.Query(It.IsAny<Expression<Func<T, bool>>>())).Returns((Expression<Func<T, bool>> predicate) => itemList.Where(predicate.Compile()).AsQueryable());
            response.Setup(moq => moq.First(It.IsAny<Expression<Func<T, bool>>>())).Returns((Expression<Func<T, bool>> predicate) => itemList.Where(predicate.Compile()).First());
            response.Setup(moq => moq.FirstOrDefault(It.IsAny<Expression<Func<T, bool>>>())).Returns((Expression<Func<T, bool>> predicate) => itemList.Where(predicate.Compile()).FirstOrDefault());
            response.Setup(moq => moq.SingleOrDefault(It.IsAny<Expression<Func<T, bool>>>())).Returns((Expression<Func<T, bool>> predicate) => itemList.Where(predicate.Compile()).SingleOrDefault());
            response.Setup(moq => moq.Any(It.IsAny<Expression<Func<T, bool>>>())).Returns((Expression<Func<T, bool>> predicate) => itemList.Where(predicate.Compile()).Any());
            response.Setup(moq => moq.All()).Returns(itemList);
            response.Setup(moq => moq.SingleOrDefault(It.IsAny<Expression<Func<T, bool>>>())).Returns((Expression<Func<T, bool>> predicate) => itemList.Where(predicate.Compile()).FirstOrDefault());
            if (idFunc != null)
            {
                response.Setup(moq => moq.GetById(It.IsAny<object>()))
                    .Returns((object id) => itemList.Find(f => idFunc(f, id)));
            }
            if (equalsFunc != null)
            {
                response.Setup(moq => moq.Update(It.IsAny<T>())).Returns<T>(item => item)
                    .Callback<T>(item =>
                    {
                        var existingIndex = itemList.FindIndex(x => equalsFunc(x, item));
                        if (existingIndex > -1)
                        {
                            itemList[existingIndex] = item;
                        }
                    });
                response.Setup(moq => moq.Update(It.IsAny<T>(), It.IsAny<string>())).Returns<T, string>((item, id) => item)
                    .Callback<T, string>((item, id) =>
                    {
                        var existingIndex = itemList.FindIndex(x => equalsFunc(x, item));
                        if (existingIndex > -1)
                        {
                            itemList[existingIndex] = item;
                        }
                    });
            }
            return response;
        }
    }
}