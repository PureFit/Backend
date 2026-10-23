namespace Backend.Application.Services;

/// <summary>
/// Выполняет вызов инструмента от AI и возвращает результат в виде JSON-строки.
/// </summary>
public interface IChatToolExecutor
{
    /// <param name="toolName">Имя инструмента из ToolCallDto.Name</param>
    /// <param name="argumentsJson">JSON аргументов из ToolCallDto.ArgumentsJson</param>
    /// <param name="userId">ID пользователя для запроса данных</param>
    /// <returns>JSON-строка с результатом — отправляется обратно в AI как tool result</returns>
    Task<string> ExecuteAsync(string toolName, string argumentsJson, Guid userId);
}
