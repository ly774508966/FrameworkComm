//////////////////////////////////////////////////////////////////////////
/// This is an auto-generated script, please do not modify it manually ///
//////////////////////////////////////////////////////////////////////////

using Framework;

namespace Project
{
    public static class E2JHashHelper
    {
        public static E2JLoader CreateLoaderByHash(uint hash)
        {
            E2JLoader loader = null;

            switch (hash)
            {
                case 1413216339:
                    {
                        loader = new E2JTableList();
                    }
                    break;
                case 1309828047:
                    {
                        loader = new E2JConfig();
                    }
                    break;
            }

            return loader;
        }
    }
}
