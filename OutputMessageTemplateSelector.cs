namespace IKSPronounceApp;

/// <summary>
/// Returns a matching data template for display of a given output message type
/// </summary>
internal class OutputMessageTemplateSelector : DataTemplateSelector
{
    public DataTemplate InformationMessageTemplate { get; set; }
    public DataTemplate ScoreMessageTemplate { get; set; }
    public DataTemplate ErrorMessageTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (item is not OutputMessage)
        {
            throw new ArgumentException("Type of argument 'item' must be 'OutputMessage'.");
        }

        var type = ((OutputMessage)item).Type;
        return type switch
        {
            OutputMessageType.Information => InformationMessageTemplate,
            OutputMessageType.Score => ScoreMessageTemplate,
            OutputMessageType.Error => ErrorMessageTemplate,
            _ => throw new ArgumentException($"OutputMessageType '{type}' unknown."),
        };
    }
}
