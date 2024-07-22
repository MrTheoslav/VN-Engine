using System.Linq;
using UnityEngine;

public class VNMenuManager : MonoBehaviour
{
    public static VNMenuManager instance;

    private MenuPage activePage = null;
    private bool isOpen = false;

    [SerializeField] private CanvasGroup root;
    [SerializeField] private MenuPage[] pages;

    private CanvasGroupController rootCG;
    private UIConfirmationMenu uiChoiceMenu => UIConfirmationMenu.instance;


    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        rootCG = new CanvasGroupController(this, root);
    }

    private MenuPage GetPage(MenuPage.PageType pageType)
    {
        return pages.FirstOrDefault(page => page.pageType == pageType);
    }

    public void OpenSavePage()
    {
        var page = GetPage(MenuPage.PageType.SaveAndLoad);
        var slm = page.anim.GetComponentInParent<SaveAndLoadMenu>();
        slm.menuFunction = SaveAndLoadMenu.MenuFunction.save;
        OpenPage(page);
    }

    public void OpenLoadPage()
    {
        var page = GetPage(MenuPage.PageType.SaveAndLoad);
        var slm = page.anim.GetComponentInParent<SaveAndLoadMenu>();
        slm.menuFunction = SaveAndLoadMenu.MenuFunction.load;
        OpenPage(page);
    }

    public void OpenConfigPage()
    {
        var page = GetPage(MenuPage.PageType.Config);
        OpenPage(page);
    }

    public void OpenHelpPage()
    {
        var page = GetPage(MenuPage.PageType.Help);
        OpenPage(page);
    }

    private void OpenPage(MenuPage page)
    {
        if (page == null)
            return;

        if (activePage != null && activePage != page)
            activePage.Close();

        page.Open();
        activePage = page;

        if (!isOpen)
            OpenRoot();
    }

    public void OpenRoot()
    {
        rootCG.Show();
        rootCG.SetInteracableState(true);
        isOpen = true;
    }

    public void CloseRoot()
    {
        rootCG.Hide();
        rootCG.SetInteracableState(false);
        isOpen = false;
    }

    public void Clicked_Home()
    {
        VN_Configuration.activeConfig.Save();

        UnityEngine.SceneManagement.SceneManager.LoadScene(MainMenu.MAIN_MENU_SCENE);
    }

    public void Clicked_Quit()
    {
        uiChoiceMenu.Show("Quit to desktop?", new UIConfirmationMenu.ConfirmationButton("Yes", () => Application.Quit()), new UIConfirmationMenu.ConfirmationButton("No", null));
    }
}
