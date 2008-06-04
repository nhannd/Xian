using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;

namespace ClearCanvas.Ris.Client
{
	public class ExternalPractitionerContactPointLookupHandler : ILookupHandler
	{
		class ContactPointSuggestionProvider : SuggestionProviderBase<ExternalPractitionerContactPointDetail>
		{
			private readonly List<ExternalPractitionerContactPointDetail> _contactPoints = new List<ExternalPractitionerContactPointDetail>();

			public ContactPointSuggestionProvider(IEnumerable<ExternalPractitionerContactPointDetail> contactPoints)
			{
				_contactPoints = new List<ExternalPractitionerContactPointDetail>(contactPoints);

				// sort results in the way that they will be formatted for the suggest box
				_contactPoints.Sort(
					delegate(ExternalPractitionerContactPointDetail x, ExternalPractitionerContactPointDetail y)
					{
						return ExternalPractitionerContactPointLookupHandler.FormatItem(x).CompareTo(ExternalPractitionerContactPointLookupHandler.FormatItem(y));
					});
			}

			protected override IList<ExternalPractitionerContactPointDetail> GetShortList(string query)
			{
				return _contactPoints;
			}

			protected override string FormatItem(ExternalPractitionerContactPointDetail item)
			{
				return ExternalPractitionerContactPointLookupHandler.FormatItem(item);
			}
		}

		private readonly EntityRef _practitionerRef;
		private readonly IList<ExternalPractitionerContactPointDetail> _contactPoints;
		private readonly IDesktopWindow _desktopWindow;
		private ContactPointSuggestionProvider _suggestionProvider;

		public ExternalPractitionerContactPointLookupHandler(
			EntityRef practitionerRef,
			IList<ExternalPractitionerContactPointDetail> contactPoints,
			IDesktopWindow desktopWindow)
		{
			_practitionerRef = practitionerRef;
			_contactPoints = contactPoints;
			_desktopWindow = desktopWindow;
		}

		private static string FormatItem(ExternalPractitionerContactPointDetail cp)
		{
			return string.Format("{0} ({1})", cp.Name, cp.Description);
		}

		#region ILookupHandler Members

		bool ILookupHandler.Resolve(string query, bool interactive, out object result)
		{
			result = null;

			ExternalPractitionerDetail practitionerDetail = null;
			Platform.GetService<IExternalPractitionerAdminService>(
				delegate(IExternalPractitionerAdminService service)
				{
					LoadExternalPractitionerForEditResponse response = service.LoadExternalPractitionerForEdit(new LoadExternalPractitionerForEditRequest(_practitionerRef));
					practitionerDetail = response.PractitionerDetail;
				});

			ExternalPractitionerContactPointSummaryComponent component = new ExternalPractitionerContactPointSummaryComponent(_practitionerRef);

			practitionerDetail.ContactPoints.ForEach(delegate(ExternalPractitionerContactPointDetail p)
													  {
														  component.Subject.Add(p);
													  });

			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				_desktopWindow, component, SR.TitleContactPoints);

			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				result = component.SummarySelection.Item;
			}

			return (result != null);
		
		}

		string ILookupHandler.FormatItem(object item)
		{
			return FormatItem((ExternalPractitionerContactPointDetail)item);
		}

		ISuggestionProvider ILookupHandler.SuggestionProvider
		{
			get
			{
				if (_suggestionProvider == null)
				{
					_suggestionProvider = new ContactPointSuggestionProvider(_contactPoints);
				}
				return _suggestionProvider;
			}
		}

		#endregion
	}
}
