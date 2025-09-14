namespace Softhand.Platforms.Android;

class SurfaceCallback(CallPageHandler handler) : Java.Lang.Object, ISurfaceHolderCallback
{
    public void SurfaceChanged(ISurfaceHolder holder, Format format, int width, int height)
    {
        handler.OnSurfaceChanged(holder, format, width, height);
    }

    public void SurfaceCreated(ISurfaceHolder holder)
    {
        handler.OnSurfaceCreated(holder);
    }

    public void SurfaceDestroyed(ISurfaceHolder holder)
    {
        handler.OnSurfaceDestroyed(holder);
    }
}