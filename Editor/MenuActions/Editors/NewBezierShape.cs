using UnityEngine.ProBuilder;
using UnityEditor.ProBuilder;
using UnityEngine;
using UnityEditor;
using UnityEditor.ProBuilder.UI;
using EditorUtility = UnityEditor.ProBuilder.EditorUtility;

namespace UnityEditor.ProBuilder.Actions
{
	class NewBezierShape : MenuAction
	{
		public override ToolbarGroup group { get { return ToolbarGroup.Tool; } }
		public override Texture2D icon { get { return IconUtility.GetIcon("Toolbar/NewBezierSpline", IconSkin.Pro); } }
		public override TooltipContent tooltip { get { return _tooltip; } }
		public override string menuTitle { get { return "New Bezier Shape"; } }
		public override int toolbarPriority { get { return 1; } }
		private bool m_ExperimentalFeaturesEnabled = false;
		public override bool isProOnly { get { return true; } }

		public NewBezierShape()
		{
			m_ExperimentalFeaturesEnabled = PreferencesInternal.GetBool(PreferenceKeys.pbEnableExperimental);
		}

		static readonly TooltipContent _tooltip = new TooltipContent
		(
			"New Bezier Shape",
			"Creates a new shape that is built by extruding along a bezier spline."
		);

		public override bool IsHidden()
		{
			return !m_ExperimentalFeaturesEnabled;
		}

		public override bool IsEnabled()
		{
			return true;
		}

		public override ActionResult DoAction()
		{
			GameObject go = new GameObject();
			BezierShape bezier = go.AddComponent<BezierShape>();
			bezier.Init();
			ProBuilderMesh pb = bezier.gameObject.AddComponent<ProBuilderMesh>();
			bezier.Refresh();
			EditorUtility.InitObject(pb);
			MeshSelection.SetSelection(go);
			UndoUtility.RegisterCreatedObjectUndo(go, "Create Bezier Shape");
			bezier.isEditing = true;

			return new ActionResult(Status.Success, "Create Bezier Shape");
		}
	}
}
