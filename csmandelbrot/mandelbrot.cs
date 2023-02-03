namespace csmandelbrot;
using static SDL2.SDL;
using System.Drawing;
public class mandelbrot
{
    bool running = false;
    IntPtr window;
    IntPtr renderer;
    
    static void Main(){
        new mandelbrot().run();
    }
    mandelbrot(){
        System.Console.WriteLine("starting");
         if (SDL_Init(SDL_INIT_VIDEO) < 0)
            {
                Console.WriteLine("SDL_Init Error: " + SDL_GetError());
                return;
            }

            window = SDL_CreateWindow("Mandelbrot render", SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, 640, 480, SDL_WindowFlags.SDL_WINDOW_SHOWN);
            if (window == IntPtr.Zero)
            {
                Console.WriteLine("SDL_CreateWindow Error: " + SDL_GetError());
                SDL_Quit();
                return;
            }

            renderer = SDL_CreateRenderer(window, -1, 0);
            if (renderer == IntPtr.Zero)
            {
                Console.WriteLine("SDL_CreateRenderer Error: " + SDL_GetError());
                SDL_DestroyWindow(window);
                SDL_Quit();
                return;
            }
            running = true;
    }

    public void run(){
    System.Console.WriteLine("starting render trhead");

    Thread renderThread = new Thread(rendermandelbrot);
    renderThread.Start();

        System.Console.WriteLine("loop initializing");
        while(running){
            poolEvents();
        }

        SDL_DestroyWindow(window);
        SDL_DestroyRenderer(renderer);
        SDL_Quit();
    }
    void poolEvents(){
        while (SDL_PollEvent(out SDL_Event e) == 1)
        {

            switch (e.type)
            {
                case SDL_EventType.SDL_QUIT:
                    running = false;
                    break;
                case SDL_EventType.SDL_KEYDOWN:
                    var key = e.key.keysym.scancode;

                    if(key == SDL_GetScancodeFromKey(SDL_Keycode.SDLK_RIGHT)){
                        posX += 0.025;
                    }
                    if(key == SDL_GetScancodeFromKey(SDL_Keycode.SDLK_LEFT)){
                        posX -= 0.025;
                    }
                    if(key == SDL_GetScancodeFromKey(SDL_Keycode.SDLK_UP)){
                        posY -= 0.025;
                    }
                    if(key == SDL_GetScancodeFromKey(SDL_Keycode.SDLK_DOWN)){
                        posY += 0.025;
                    }
                    //equals is for keyboards without numeric keyboard
                    if(key == SDL_GetScancodeFromKey(SDL_Keycode.SDLK_EQUALS) || key == SDL_GetScancodeFromKey(SDL_Keycode.SDLK_PLUS)){
                        zoom += 0.1;
                    }
                     if(key == SDL_GetScancodeFromKey(SDL_Keycode.SDLK_MINUS)){
                        zoom -= 0.1;
                    }

                    break;
                   
            }
        }
    }

    public double zoom = 1, posX = -0.6, posY = 0;
    public int maxIterations = 255;
   void rendermandelbrot(){
    while (running)
    {
        int width = 640;
        int height = 480;

        Color[] colors = new Color[maxIterations];
       for (int i = 0; i < maxIterations; i++)
        {
            float hue = (float)(i / (float)maxIterations);
            colors[i] = ColorFromHSV(hue, 1, 1);
        }



        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                double zx = 1.25 * (x - width / 2) / (0.5 * zoom * width) + posX;
                double zy = (y - height / 2) / (0.5 * zoom * height) + posY;
                double cX = zx;
                double cY = zy;
                int iterations = 0;

                while (iterations < maxIterations && (zx * zx + zy * zy) < 4)
                {
                    double newzx = zx * zx - zy * zy + cX;
                    double newzy = 2 * zx * zy + cY;
                    zx = newzx;
                    zy = newzy;
                    iterations++;
                }

                Color colorValue = colors[iterations % maxIterations];
                SDL_SetRenderDrawColor(renderer, colorValue.R, colorValue.G, colorValue.B, 255);
                SDL_RenderDrawPoint(renderer, x, y);
            }
        }
        SDL_RenderPresent(renderer);
    }
}

    Color ColorFromHSV(float hue, float saturation, float value)
    {
        int h = Convert.ToInt32(Math.Floor(hue * 6)) % 6;
        float f = hue * 6 - Convert.ToSingle(Math.Floor(hue * 6));
        float p = value * (1 - saturation);
        float q = value * (1 - f * saturation);
        float t = value * (1 - (1 - f) * saturation);
         switch (h)
        {
            case 0:
                return Color.FromArgb((int)(255 * value), 0, 0, (int)(255 * t));
            case 1:
                return Color.FromArgb((int)(255 * q), (int)(255 * value), 0, 0);
            case 2:
                return Color.FromArgb(0, (int)(255 * value), (int)(255 * t), 0);
            default:
                return Color.FromArgb(0, (int)(255 * value), (int)(255 * t), 0);
        }
    }
}