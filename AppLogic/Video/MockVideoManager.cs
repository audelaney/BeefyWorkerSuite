using DataObjects;

namespace AppLogic
{
	internal class MockVideoManager : VideoManager
	{
		public MockVideoManager()
		{
		}

		public override Scene[] GetScenesFromVideo(string videoPath)
		{
			var scenes = new Scene[10];

			for (int i = 0; i < scenes.Length; i++)
			{ scenes[i] = new Scene(i * 10, (i + 1) * 10); }

			return scenes;
		}
	}
}