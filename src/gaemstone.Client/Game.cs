using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using gaemstone.Client.Components;
using gaemstone.Client.Graphics;
using gaemstone.Client.Processors;
using gaemstone.Common.Components;
using gaemstone.Common.ECS;
using gaemstone.Common.ECS.Stores;
using Silk.NET.Windowing.Common;

namespace gaemstone.Client
{
	public abstract class Game : Universe
	{
		public IWindow Window { get; }

		public Game()
		{
			Window = Silk.NET.Windowing.Window.Create(new WindowOptions {
				Title = "gæmstone",
				Size  = new Size(1280, 720),
				API   = GraphicsAPI.Default,
				UpdatesPerSecond = 30.0,
				FramesPerSecond  = 60.0,
				ShouldSwapAutomatically = true,
			});
			Window.Load    += OnLoad;
			Window.Update  += OnUpdate;
			Window.Closing += OnClosing;

			Components.AddStore(new PackedArrayStore<Transform>());
			Components.AddStore(new PackedArrayStore<IndexedMesh>());
			Components.AddStore(new PackedArrayStore<Texture>());
			Components.AddStore(new DictionaryStore<Camera>());
		}

		public void Run()
		{
			Window.Run();
		}


		protected virtual void OnLoad()
		{
			GFX.Initialize();
			GFX.OnDebugOutput += (source, type, id, severity, message) =>
				Console.WriteLine($"[GLDebug] [{severity}] {type}/{id}: {message}");

			Processors.Start<Renderer>();
			Processors.Start<TextureManager>();
			Processors.Start<MeshManager>();
			Processors.Start<CameraController>();

			var mainCamera = Entities.New();
			Set(mainCamera, (Transform)Matrix4x4.Identity);
			Set(mainCamera, Camera.Default3D);
		}

		protected virtual void OnClosing()
		{

		}

		protected virtual void OnUpdate(double delta)
		{
			foreach (var processor in Processors)
				processor.OnUpdate(delta);
		}


		public abstract Stream GetResourceStream(string name);

		public string GetResourceAsString(string name)
		{
			using (var stream = GetResourceStream(name))
			using (var reader = new StreamReader(stream))
				return reader.ReadToEnd();
		}
	}
}
