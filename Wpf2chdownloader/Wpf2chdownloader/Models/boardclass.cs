


namespace Wpf2chdownloader.Models.boardclass
{
    public class Rootobject
    {
        public string Board { get; set; }
        public string BoardInfo { get; set; }
        public string BoardInfoOuter { get; set; }
        public string BoardName { get; set; }
        public string board_banner_image { get; set; }
        public string board_banner_link { get; set; }
        public int bump_limit { get; set; }
        public string default_name { get; set; }
        public int enable_dices { get; set; }
        public int enable_flags { get; set; }
        public int enable_icons { get; set; }
        public int enable_images { get; set; }
        public int enable_likes { get; set; }
        public int enable_names { get; set; }
        public int enable_oekaki { get; set; }
        public int enable_posting { get; set; }
        public int enable_sage { get; set; }
        public int enable_shield { get; set; }
        public int enable_subject { get; set; }
        public int enable_thread_tags { get; set; }
        public int enable_trips { get; set; }
        public int enable_video { get; set; }
        public string filter { get; set; }
        public int max_comment { get; set; }
        public int max_files_size { get; set; }
        public int max_vip_files { get; set; }
        public int max_vip_files_size { get; set; }
        public News[] news { get; set; }
        public Thread[] threads { get; set; }
        public Top[] top { get; set; }
    }

    public class News
    {
        public string date { get; set; }
        public string num { get; set; }
        public string subject { get; set; }
    }

    public class Thread
    {
        public int banned { get; set; }
        public int closed { get; set; }
        public string comment { get; set; }
        public string date { get; set; }
        public string email { get; set; }
        public File[] files { get; set; }
        public int files_count { get; set; }
        public double lasthit { get; set; }
        public string name { get; set; }
        public string num { get; set; }
        public int op { get; set; }
        public string parent { get; set; }
        public int posts_count { get; set; }
        public int sticky { get; set; }
        public string subject { get; set; }
        public string tags { get; set; }
        public double timestamp { get; set; }
        public string trip { get; set; }
        public string trip_type { get; set; }
    }

    public class File
    {
        public int height { get; set; }
        public string md5 { get; set; }
        public string name { get; set; }
        public int nsfw { get; set; }
        public string path { get; set; }
        public int size { get; set; }
        public string thumbnail { get; set; }
        public int tn_height { get; set; }
        public int tn_width { get; set; }
        public int type { get; set; }
        public int width { get; set; }
        public string duration { get; set; }
    }

    public class Top
    {
        public string board { get; set; }
        public string info { get; set; }
        public string name { get; set; }
        public int speed { get; set; }
    }


}
