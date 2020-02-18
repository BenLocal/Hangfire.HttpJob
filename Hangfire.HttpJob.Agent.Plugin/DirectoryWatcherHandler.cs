using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

namespace Hangfire.HttpJob.Agent.Plugin
{
	public class DirectoryWatcherHandler
	{
		private readonly PluginContext _pluginContext;

		public DirectoryWatcherHandler(PluginContext pluginContext)
		{
			_pluginContext = pluginContext;
		}

		public void OnEvent(object source, FileSystemEventArgs args)
		{
			// 新添加文件
			if (args.ChangeType == WatcherChangeTypes.Created)
			{
				_pluginContext.Reload(args.FullPath);
			}

			Console.Out.WriteLine(args.ChangeType.ToString());
		}

		public void OnEventError(object source, ErrorEventArgs args)
		{
			Console.Out.WriteLine(args.ToString());
		}
	}
}
