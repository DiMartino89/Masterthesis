using CSharpCompiler;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CodingBoxController : MonoBehaviour, ICodingBoxController
{
    private const string FILE_SUBFOLDER = "UserScripts";

    [SerializeField]
    private bool _autoSave = true;

    [SerializeField]
    private SyntaxColors _syntaxColors;

    [SerializeField]
    private TMP_InputField _codingBoxInputField;

    [SerializeField]
    private TMP_InputField _fileNameInputField;

    [SerializeField]
    private TMP_InputField _componentParentName;

    [SerializeField]
    private GameObject _defaultComponentParent;

    private DeferredSynchronizeInvoke _synchronizedInvoke;
    private ScriptBundleLoader _loader;

    private FileController _fileController;
    private UnitySyntaxHighlighter _unitySyntaxHighlighter;

    private string _linesOfCodeOnTop;
    private string _codeToShow;
    private string _linesOfCodeOnBottom;
    private int _caretPositionOnHighlighting;
    private Color _caretColorOnHighlighting;
    private Coroutine _highlightCoroutine;

    private readonly Dictionary<GameObject, Dictionary<string, Component>> _gameobjectComponents
        = new Dictionary<GameObject, Dictionary<string, Component>>();

    public IDictionary<GameObject, Dictionary<string, Component>> GameobjectComponents { get { return _gameobjectComponents; } }

    [Serializable]
    public class SyntaxColors
    {
        public Color CommentColor = Color.white;
        public Color KeywordColor = Color.white;
        public Color QuoteColor = Color.white;
        public Color TypeColor = Color.white;
        public Color MethodColor = Color.white;
        public Color NumberColor = Color.white;
    }

    private string ComponentName
    {
        get
        {
            if (_fileNameInputField != null && string.IsNullOrEmpty(_fileNameInputField.text))
            {
                return _fileNameInputField.text;
            }

            return "TestScript.cs";
        }
    }

    private GameObject ComponentParent
    {
        get
        {
            if (_componentParentName != null && !string.IsNullOrEmpty(_componentParentName.text))
            {
                var parent = GameObject.Find(_componentParentName.text);

                if (parent != null)
                {
                    return parent;
                }
                else
                {
                    Debug.LogError("Can not find gameobject with name: " + _componentParentName.name);
                }
            }

            return _defaultComponentParent != null ? _defaultComponentParent : gameObject;
        }
    }

    public void Load()
    {
        Load(_fileNameInputField.text);
    }
    public void Load(string fileName = "", int hiddenLinesOfCodeOnTop = 0, int hiddenLinesOfCodeOnBottom = 0)
    {
        string fileContent = _fileController.LoadTextFromFile(fileName);

        int topSplitIndex = 0;
        for (int i = 0; i < hiddenLinesOfCodeOnTop; i++)
        {
            topSplitIndex = fileContent.IndexOf('\n' /*Environment.NewLine*/, topSplitIndex + 1);
        }

        int bottomSplitIndex = fileContent.Length - 1;
        for (int i = 0; i < hiddenLinesOfCodeOnBottom; i++)
        {
            bottomSplitIndex = fileContent.Substring(0, bottomSplitIndex).LastIndexOf('\n'/*Environment.NewLine*/);
        }

        _linesOfCodeOnTop = fileContent.Substring(0, topSplitIndex);
        _codeToShow = fileContent.Substring(topSplitIndex, bottomSplitIndex - topSplitIndex);
        _linesOfCodeOnBottom = fileContent.Substring(bottomSplitIndex, fileContent.Length - bottomSplitIndex);

        HighlightCode(_codeToShow);
    }

    public void Save()
    {
        string codeToSave = _linesOfCodeOnTop + _unitySyntaxHighlighter.getCodeWithoutRichText(_codingBoxInputField.text) + _linesOfCodeOnBottom;
        _fileController.SaveTextToFile(_fileNameInputField.text, codeToSave);
    }

    public void Run()
    {
        if (_autoSave)
        {
            Save();
        }

        var path = _fileController.GetFilePath(_fileNameInputField.text);

        if (!string.IsNullOrEmpty(path))
        {
            _loader.LoadAndWatchScriptsBundle(new[] { path });
        }
        else
        {
            Debug.LogError("Can not run file " + _fileNameInputField.text);
        }
    }

    public void EnableComponent(Component component)
    {
        if (component != null && component is MonoBehaviour) (component as MonoBehaviour).enabled = true;
    }
    public void EnableComponent(string componentName)
    {
        foreach (var dicts in _gameobjectComponents.Values)
        {
            Component component;
            if (dicts.TryGetValue(componentName, out component))
            {
                EnableComponent(component);
                return;
            }
        }
    }

    public void DisableComponent(Component component)
    {
        if (component != null && component is MonoBehaviour) (component as MonoBehaviour).enabled = false;
    }
    public void DisableComponent(string componentName)
    {
        foreach (var dicts in _gameobjectComponents.Values)
        {
            Component component;
            if (dicts.TryGetValue(componentName, out component))
            {
                DisableComponent(component);
                return;
            }
        }
    }

    public void DestroyComponent(Component component)
    {
        _loader.destroyInstance(component);
    }
    public void DestroyComponent(string componentName)
    {
        foreach (var dicts in _gameobjectComponents.Values)
        {
            Component component;
            if (dicts.TryGetValue(componentName, out component))
            {
                DestroyComponent(component);
                return;
            }
        }
    }

    public void BlockUserInputForCodingBox()
    {
        _codingBoxInputField.readOnly = true;
    }

    public void UnblockUserInputForCodingBox()
    {
        _codingBoxInputField.readOnly = false;
    }

    public void WriteToCodingBox(string text)
    {
        _codingBoxInputField.text += text;
    }

    private void Awake()
    {
        _fileController = new FileController(FILE_SUBFOLDER);

        _unitySyntaxHighlighter = new UnitySyntaxHighlighter(new UnitySyntaxHighlighter.SyntaxColors()
        {
            CommentColor = _syntaxColors.CommentColor.ColorToHex(),
            KeywordColor = _syntaxColors.KeywordColor.ColorToHex(),
            QuoteColor = _syntaxColors.QuoteColor.ColorToHex(),
            TypeColor = _syntaxColors.TypeColor.ColorToHex(),
            MethodColor = _syntaxColors.MethodColor.ColorToHex(),
            NumberColor = _syntaxColors.NumberColor.ColorToHex()
        });

        _codingBoxInputField.onValueChanged.AddListener(ResetCodeHighlighting);

        _synchronizedInvoke = new DeferredSynchronizeInvoke();
        _loader = new ScriptBundleLoader(_synchronizedInvoke)
        {
            logWriter = new UnityLogTextWriter(),
            createInstance = (Type type) =>
            {
                if (typeof(Component).IsAssignableFrom(type))
                {
                    var componentParent = ComponentParent;
                    var components = _gameobjectComponents.GetOrAdd(componentParent, new Dictionary<string, Component>());

                    string componentName = type.Name;// _fileNameInputField.text;
                    Component component;
                    if (components.TryGetValue(componentName, out component))
                    {
                        Destroy(component);
                    }

                    component = componentParent.AddComponent(type);

                    components.Update(componentName, component);

                    return component;
                }
                else
                {
                    return Activator.CreateInstance(type);
                }
            },
            destroyInstance = (object instance) =>
            {
                if (instance != null && instance is Component)
                {
                    if (instance is MonoBehaviour)
                    {
                        var component = (instance as MonoBehaviour);
                        Dictionary<string, Component> components;
                        if (_gameobjectComponents.TryGetValue(component.gameObject, out components))
                        {
                            components.Remove(component.GetType().ToString());
                        }

                        Destroy(component);
                    }
                }
            }
        };
    }

    private void HighlightCode(string content)
    {
        _highlightCoroutine = StartCoroutine(_unitySyntaxHighlighter.HighlightSourceCoroutine(content, OnCodingTextHighlighted));
    }

    public void ResetCodeHighlighting(string content)
    {
        if (_highlightCoroutine != null)
        {
            StopCoroutine(_highlightCoroutine);
        }

        HighlightCode(_unitySyntaxHighlighter.getCodeWithoutRichText(_codingBoxInputField.text));
    }

    private void OnCodingTextHighlighted(string content)
    {
        if (!_codingBoxInputField.text.Equals(content))
        {
            _codingBoxInputField.onValueChanged.RemoveListener(ResetCodeHighlighting);
            _codingBoxInputField.readOnly = true;

            _caretPositionOnHighlighting = _codingBoxInputField.caretPosition;
            _caretColorOnHighlighting = _codingBoxInputField.caretColor;
            _codingBoxInputField.caretColor = new Color(0, 0, 0, 0);

            int textLength = _codingBoxInputField.text.Length;
            _codingBoxInputField.text = content;
            Debug.Log("update code");
            StartCoroutine(SetCaret(content.Length >= textLength ? 1 : 0));
        }
    }

    private System.Collections.IEnumerator SetCaret(int offset)
    {
        yield return new WaitForEndOfFrame();

        _codingBoxInputField.caretPosition = _caretPositionOnHighlighting + offset;

        _codingBoxInputField.caretColor = _caretColorOnHighlighting;

        _codingBoxInputField.onValueChanged.AddListener(ResetCodeHighlighting);
        _codingBoxInputField.readOnly = false;

        _highlightCoroutine = null;
    }
}
