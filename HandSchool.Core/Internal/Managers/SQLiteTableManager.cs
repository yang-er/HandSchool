using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using SQLite;
using Xamarin.Forms.Internals;

namespace HandSchool.Internals
{
    public class SQLiteResult<T>
    {
        public bool AllSuccess => _failedItems is null;
        public int SuccessCount { get; set; }
        public IReadOnlyCollection<KeyValuePair<T, SQLiteException>> FailedItems => _failedItems;
        private List<KeyValuePair<T, SQLiteException>> _failedItems;

        public void Add(T key, SQLiteException value)
        {
            _failedItems ??= new List<KeyValuePair<T, SQLiteException>>();
            _failedItems.Add(new KeyValuePair<T, SQLiteException>(key, value));
        }
    }

    public class SQLiteTableManager<TTable> where TTable : class, new()
    {
        
        private readonly IReadOnlyList<string> _paths;
        private readonly string _rootDir;
        private bool _autoCreate;
        private bool _tableCreated;
        public string TableName { get; }
        public string DataBasePath { get; }
        private string PrimaryKey { get; }
        public string PrimaryKeyColName { get; }

        private bool HasFile()
        {
            return File.Exists(DataBasePath);
        }
        public bool HasTable()
        {
            if (!HasFile()) return false;
            using var connection = new SQLiteConnection(DataBasePath);
            var info = connection.GetTableInfo(TableName);
            return info.Count != 0;
        }

        /// <summary>
        /// SQLite表管理器
        /// </summary>
        /// <param name="autoCreate">是否自动创建文件和表</param>
        /// <param name="rootDir">SQLite文件的根目录，一般为包名目录</param>
        /// <param name="paths">SQLite文件的路径，拼合在主目录之后</param>
        public SQLiteTableManager(bool autoCreate, string rootDir, params string[] paths)
        {
            //获取表的信息
            var type = typeof(TTable);
            var attr = type.GetCustomAttribute(typeof(TableAttribute), false) as TableAttribute;
            TableName = attr == null ? type.Name : attr.Name;

            //获取主键信息
            var props = type.GetProperties();
            var pkInfo =
                (from p in props
                    where p.GetCustomAttribute(typeof(PrimaryKeyAttribute)) != null
                    select p).FirstOrDefault();
            PrimaryKey = pkInfo?.Name;
            var pkc = pkInfo?.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute;
            PrimaryKeyColName = pkc?.Name ?? PrimaryKey;
            
            //计算SQLite文件的路径
            _rootDir = rootDir;
            var ps = new List<string>();
            Array.ForEach(paths, s =>
            {
                var ss = s.Split(Path.DirectorySeparatorChar)
                    .Where(str => !string.IsNullOrWhiteSpace(str));
                ps.AddRange(ss);
            });
            _paths = ps;
            
            //合并SQLite文件的路径
            var dbPath = rootDir;
            _paths.ForEach(p => dbPath = Path.Combine(dbPath, p));
            DataBasePath = dbPath;

            _autoCreate = autoCreate;
            if (autoCreate)
            {
                CreateTable();
                _tableCreated = true;
            }
            else
            {
                _tableCreated = HasTable();
            }
        }
        
        private void CreateDirs()
        {
            if (_rootDir != "" && !Directory.Exists(_rootDir))
            {
                throw new ArgumentException("The root directory does not exist! ");
            }
            var path = _rootDir;
            for (var i = 0; i < _paths.Count - 1; i++)
            {
                path = Path.Combine(path, _paths[i]);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
        }
        public void CreateTable()
        {
            CreateDirs();
            using var connection = new SQLiteConnection(DataBasePath);
            connection.CreateTable(typeof(TTable));
            _tableCreated = true;
        }
        private void CheckTable()
        {
            if (_autoCreate) return;
            if (_tableCreated) return;
            if (!HasTable())
            {
                throw new InvalidOperationException(
                    $"AutoCreate is \"false\" and the table:\"{TableName}\" does not exist! Call \"CreateTable()\" before this.");
            }
            else
            {
                _tableCreated = true;
            }
        }

        public SQLiteResult<TTable> UpdateTable(params TTable[] objs)
        {
            CheckTable();
            using var connection = new SQLiteConnection(DataBasePath);
            var res = new SQLiteResult<TTable> {SuccessCount = 0};
            Array.ForEach(objs, o =>
            {
                try
                {
                    if (connection.Update(o) == 1)
                    {
                        res.SuccessCount++;
                    }
                    else
                    {
                        res.Add(o, null);
                    }
                }
                catch (SQLiteException e)
                {
                    res.Add(o, e);
                }
            });
            return res;
        }

        public SQLiteResult<TTable> InsertTable(params TTable[] objs)
        {
            CheckTable();
            using var connection = new SQLiteConnection(DataBasePath);
            var res = new SQLiteResult<TTable> {SuccessCount = 0};
            Array.ForEach(objs, o =>
            {
                try
                {
                    if (connection.Insert(o) == 1)
                    {
                        res.SuccessCount++;
                    }
                    else
                    {
                        res.Add(o, null);
                    }
                }
                catch (SQLiteException e)
                {
                    res.Add(o, e);
                }
            });
            return res;
        }

        public SQLiteResult<TTable> InsertOrUpdateTable(params TTable[] objs)
        {
            CheckTable();
            using var connection = new SQLiteConnection(DataBasePath);

            var res = new SQLiteResult<TTable> {SuccessCount = 0};
            Array.ForEach(objs, o =>
            {
                try
                {
                    if (connection.Insert(o) == 1)
                    {
                        res.SuccessCount++;
                    }
                    else
                    {
                        res.Add(o, null);
                    }
                }
                catch (SQLiteException e)
                {
                    if (e.Result == SQLite3.Result.Constraint)
                    {
                        if (connection.Update(o) == 1)
                        {
                            res.SuccessCount++;
                        }
                        else
                        {
                            res.Add(o, null);
                        }
                    }
                    else
                    {
                        res.Add(o, e);
                    }
                }
            });
            return res;
        }

        public List<TTable> GetItems(params KeyValuePair<string, object>[] keyValues)
        {
            CheckTable();
            using var connection = new SQLiteConnection(DataBasePath);
            string sql;
            if (keyValues.Length == 0)
            {
                sql = $"select * from \"{TableName}\"";
            }
            else
            {
                var sb = new StringBuilder($"select * from \"{TableName}\" where ");
                for (var i = 0; i < keyValues.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(keyValues[i].Key))
                    {
                        throw new ArgumentException("Col name cannot be null or blank! ");
                    }

                    sb.Append(i == 0 ? $"{keyValues[i].Key} = ? " : $"and {keyValues[i].Key} = ?");
                }

                sql = sb.ToString();
            }
            
            return connection.Query<TTable>(sql,
                keyValues.Select(kv => kv.Value).ToArray());
        }

        public int DeleteItems(params KeyValuePair<string, object>[] keyValues)
        {
            CheckTable();
            using var connection = new SQLiteConnection(DataBasePath);
            string sql;
            if (keyValues.Length == 0)
            {
                sql = $"delete from \"{TableName}\"";
            }
            else
            {
                var sb = new StringBuilder($"delete from \"{TableName}\" where ");
                for (var i = 0; i < keyValues.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(keyValues[i].Key))
                    {
                        throw new ArgumentException("Col name cannot be null or blank! ");
                    }

                    sb.Append(i == 0 ? $"{keyValues[i].Key} = ? " : $"and {keyValues[i].Key} = ?");
                }

                sql = sb.ToString();
            }

            return connection.Execute(sql,
                keyValues.Select(kv => kv.Value).ToArray());
        }

        public TTable GetItemWithPrimaryKey(object value)
        {
            CheckTable();
            if (PrimaryKey is null)
            {
                throw new InvalidOperationException("This table does not have a primary key");
            }

            var res = GetItems(new KeyValuePair<string, object>(PrimaryKeyColName, value));
            return res.Count == 0 ? null : res[0];
        }

        public bool DeleteItemWithPrimaryKey(object value)
        {
            CheckTable();
            if (PrimaryKey is null)
            {
                throw new InvalidOperationException("This table does not have a primary key");
            }

            var res = DeleteItems(new KeyValuePair<string, object>(PrimaryKeyColName, value));
            return res == 1;
        }

        public void DropTable()
        {
            using var connection = new SQLiteConnection(DataBasePath);
            connection.DropTable<TTable>();
        }
    }
}