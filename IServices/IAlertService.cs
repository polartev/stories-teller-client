using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Story_Teller.IServices;

public interface IAlertService
{
    /// <summary>
    /// Shows an alert dialog with a title and message.
    /// </summary>
    public Task ShowAlertAsync(string title, string message);

    /// <summary>
    /// Shows a confirmation dialog with a title and message, returning true if the user confirms.
    /// </summary>
    public Task<bool> ShowConfirmationAsync(string title, string message);
}
