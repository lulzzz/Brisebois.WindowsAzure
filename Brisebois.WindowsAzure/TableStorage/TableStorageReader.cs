using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Brisebois.WindowsAzure.TableStorage
{
public class TableStorageReader
{
  private readonly CloudStorageAccount storageAccount;
  private readonly CloudTableClient tableClient;
  private readonly CloudTable tableReference;
  private CacheItemPolicy cachePolicy;
  private string cacheHint = string.Empty;

  private TableStorageReader(string tableName)
  {
      var cs = CloudConfigurationManager.GetSetting("StorageConnectionString");
      storageAccount = CloudStorageAccount.Parse(cs);
      tableClient = storageAccount.CreateCloudTableClient();
      tableReference = tableClient.GetTableReference(tableName);

      var tableServicePoint = ServicePointManager
                               .FindServicePoint(storageAccount.TableEndpoint);
      tableServicePoint.UseNagleAlgorithm = false;
  }

  public TableStorageReader CreateIfNotExist()
  {
      tableReference.CreateIfNotExists();
      return this;
  }

  public async Task<ICollection<TEntity>> 
      Execute<TEntity>(CloudTableQuery<TEntity> query)
      where TEntity : ITableEntity
  {
      if (query == null)
          throw new ArgumentNullException("query");

      return await Task.Run(() =>
          {
              if (cachePolicy == null)
                  return query.Execute(tableReference);

              var cached = new CloudTableQueryCache<TEntity>(query, 
                                                             cachePolicy, 
                                                             cacheHint);
              return cached.Execute(tableReference);
          });
  }

  public static TableStorageReader Table(string tableName)
  {
      return new TableStorageReader(tableName);
  }

  //Cache for 1 minute
  public TableStorageReader WithCache()
  {
      var policy = new CacheItemPolicy
          {
              AbsoluteExpiration = DateTime.UtcNow.AddMinutes(1d)
          };
      return WithCache(policy);
  }

  public TableStorageReader WithCache(CacheItemPolicy policy)
  {
      return WithCache(policy, string.Empty);
  }

  public TableStorageReader WithCache(CacheItemPolicy policy,
                                      string cacheKey)
  {
      if (policy == null) throw new ArgumentNullException("policy");
      if (cacheKey == null) throw new ArgumentNullException("cacheKey");

      cachePolicy = policy;
      cacheHint = cacheKey;

      return this;
  }
}
}