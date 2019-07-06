using System;
using System.Windows.Forms;
using System.Drawing; 
class form : Form
{
    public form()
    {
        ClientSize = new Size(640, 640);
        MaximizeBox = MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.None;
        Opacity = 0.8;
        Location = new Point(100, 0);
        KeyUp += press;
        Paint += init;
    }
    void press(object o, KeyEventArgs e)
    {
        e.Handled = true;
        switch (e.KeyCode)
        {
            case Keys.Up: up(); break;
            case Keys.Down: down(); break;
            case Keys.Left: left(); break;
            case Keys.Right: right(); break;
            case Keys.F1: text(); return;
            case Keys.Escape: lose(); break;
            default: return;
        }
        if (!moved && !over()) return;
        if (overflow())
        {
            win(); return;
        }
        if (!produce())
        {
            if (canMove()) return;
            lose(); return;
        }
        draw();
    }
    bool canMove()
    {
        int i, j;
        for (i = 0; i < 4; i++)
            for (j = 0; j < 3;j++ )
                if (a[i, j] == a[i, j + 1]||a[j,i]==a[j+1,i])
                    return true;
        return false;
    }
    void draw()
    {
        int i, j;
        int w = ClientSize.Width / 4, h = ClientSize.Height / 4;
        Bitmap bit = new Bitmap(ClientSize.Width, ClientSize.Height);
        Graphics.FromImage(bit).Clear(Color.AliceBlue);
        for (i = 0; i < 4; i++)
            for (j = 0; j < 4; j++)
                Graphics.FromImage(bit).DrawImage(Image.FromFile(a[i, j]+".png"), new Rectangle(j* w, i * h, w - 2, h - 2));
        CreateGraphics().DrawImage(bit,0,0);
    }

    static void Main()
    {
        Application.EnableVisualStyles();
        Application.Run(new form());
    }
    int[,] a = new int[4, 4];
    bool moved;
    Random r = new Random();
    void init(object o, EventArgs e)//编程时能不调用别的函数就尽量不要调用
    //调用多了会忘记有没有调用,用许多个小函数累积起来更好
    //集中调用小函数,不要总是调用
    //一个函数一个功能
    {
        int i, j;
        for (i = 0; i < 4; i++)
            for (j = 0; j < 4; j++)
                a[i, j] = 0;
        produce();
        draw();
    }
    bool produce()
    {//每按一下键,就会产生一个新数字,这个新数字的位置是随机的
        int[,] b = new int[16, 2];//用来存储空位,这些空位可以产生新数字
        int i, j;
        int size = 0;
        for (i = 0; i < 4; i++)
            for (j = 0; j < 4; j++)
            {
                if (a[i, j] == 0)
                {
                    b[size, 0] = i;
                    b[size, 1] = j;
                    size++;
                }
            }
        if (size == 0) return false;//如果没有空位了,游戏就结束了
        i = r.Next() % size;
        if (r.Next() % 5 == 0) a[b[i, 0], b[i, 1]] = 4;
        else a[b[i, 0], b[i, 1]] = 2;
        return true;
    }
    void up()
    {//按下向上键时,需要对数字进行合并整理,这个函数是最重要的
        moved = false;
        int i, j, k; 
        for (i = 0; i < 4; i++)
        { 
            for (j = 0; j < 4; j++)
            {
                if(a[i,j]==0)continue;
                for (k = i; k > 0; k--)
                {
                    if (a[k-1, j] != 0) break;
                    a[k-1, j] = a[k, j];
                    a[k, j] = 0;
                    moved = true;
                }
            }
        }
        for (j = 0; j < 4;j++ )
            for (i = 3; i > 0; i--)
            {
                if (a[i, j] != 0 && a[i, j] == a[i - 1, j])
                {
                    a[i - 1, j] *= 2;
                    a[i, j] = 0;
                    moved = true;
                    break;
                }
            }
    }
    void clockwise()
    {//将数组表示的4*4方格顺时针转90度.一会儿会有大用.
        int i, j;
        int[,] b = new int[4, 4];
        for (i = 0; i < 4; i++)
            for (j = 0; j < 4; j++)
                b[i, j] = a[i, j];
        for (i = 0; i < 4; i++)
            for (j = 0; j < 4; j++)
                a[i, j] = b[3 - j, i];
    }
    void counterclockwise()
    {//将4*4方格逆时针转90度
        clockwise();
        clockwise();
        clockwise();
    }
    void upsidedown()
    {//将4*4方格旋转180度
        clockwise();
        clockwise();
    }
    void down()
    {//向下调整可以转化为向上调整,这样做是以牺牲效率为代价去换取编程的简洁清晰
        upsidedown();
        up();
        upsidedown();
    }
    void left() 
    {
        clockwise();
        up();
        counterclockwise();
    }
    void right()
    {
        counterclockwise();
        up();
        clockwise();
    }
    void win()
    {
        DialogResult o = new DialogResult();
        o = MessageBox.Show("您胜利了,不用再玩了!", "result", MessageBoxButtons.RetryCancel);
        if (o == DialogResult.Retry)
            init(null, null);
        else Application.Exit();
    }
    void lose()
    {//输了
        DialogResult o = new DialogResult();
        o = MessageBox.Show("您输了!", "result", MessageBoxButtons.RetryCancel);
        if (o == DialogResult.Retry)
            init(null, null);
        else Application.Exit();
    }
    void text()
    {
        MessageBox.Show("The arrow represent up,down,left and right\n"
            + "Esc is quit\n"
            + "F1 is reading description text"
            + "\n\t\tmade by weidiao.neu", "help"
            );
    }
    bool over()
    {
        int i, j;
        for (i = 0; i < 4; i++)
            for (j = 0; j < 4; j++)
                if (a[i, j] == 0) return false;
        return true;
    }
    bool overflow()
    {
        int i, j;
        for (i = 0; i < 4; i++)
            for (j = 0; j < 4; j++)
                if (a[i, j] > 4096)
                    return true;
        return false;
    }
}