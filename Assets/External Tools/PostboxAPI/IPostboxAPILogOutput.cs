namespace PostboxAPI
{
    /// <summary>
    /// Interface for Loogbook Output classes.
    /// For getting the next Message use output = PostboxLogbook.Instance.GetMessage();
    /// </summary>
    interface IPostboxAPILogOutput
    {
        /// <summary>
        /// Show the next bufferd Message of the PostboxLogbook
        /// </summary>
        void PrintNextMessage();
    }
}