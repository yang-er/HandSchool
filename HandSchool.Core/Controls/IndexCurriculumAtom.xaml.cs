using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HandSchool.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IndexCurriculumAtom : Frame
    {
        public ClassState ItemState
        {
            get =>(ClassState) GetValue(ItemStateProperty);
            set => SetValue(ItemStateProperty, value);
        }
        public string Sections
        {
            get => GetValue(SectionsProperty) as string;
            set => SetValue(SectionsProperty, value);
        }
        public string Descreption
        {
            get => GetValue(DescreptionProperty) as string;
            set => SetValue(DescreptionProperty, value);
        }
        public string Teacher
        {
            get => GetValue(TeacherProperty) as string;
            set => SetValue(TeacherProperty, value);
        }
        public string ClassRoom
        {
            get => GetValue(ClassRoomProperty) as string;
            set => SetValue(ClassRoomProperty, value);
        }
        public string Name
        {
            get => GetValue(NameProperty) as string;
            set => SetValue(NameProperty, value);
        }
        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            switch (propertyName)
            {
                case nameof(Name):
                    name.Text = Name.Length < 20 ? Name : Name.Substring(0, 18) + "..."; break;
                case nameof(Sections):
                    sections.Text = Sections; break;
                case nameof(ClassRoom):
                    classRoom.Text = ClassInfoSimplifier.Instance.SimplifyName(ClassRoom).Replace('\n', ' '); break;
                case nameof(Teacher):
                    teacher.Text = Teacher; break;
                case nameof(Descreption):
                    desc.Text = Descreption; break;
                case nameof(ItemState):
                    switch (ItemState)
                    {
                        case ClassState.Other: desc.Text = ""; break;
                        case ClassState.Current: desc.Text = "现在"; break;
                        case ClassState.Next: desc.Text = "接下来"; break;
                    }
                    break;

                case nameof(IsSelected):
                    if (IsSelected)
                        ViewExtensions.ScaleTo(this, 0.97);
                    else ViewExtensions.ScaleTo(this, 0.90);
                    break;

                default:
                    base.OnPropertyChanged(propertyName);
                    break;
            }
        }
        public static BindableProperty SectionsProperty =
            BindableProperty.Create(
                propertyName: nameof(Sections),
                returnType: typeof(string),
                defaultValue: string.Empty,
                declaringType: typeof(IndexCurriculumAtom),
                defaultBindingMode: BindingMode.OneWay
            );

        public static BindableProperty DescreptionProperty =
            BindableProperty.Create(
                propertyName: nameof(Descreption),
                returnType: typeof(string),
                defaultValue: string.Empty,
                declaringType: typeof(IndexCurriculumAtom),
                defaultBindingMode: BindingMode.OneWay
            );
        public static BindableProperty NameProperty =
            BindableProperty.Create(
                propertyName: nameof(Name),
                returnType: typeof(string),
                defaultValue: string.Empty,
                declaringType: typeof(IndexCurriculumAtom),
                defaultBindingMode: BindingMode.OneWay
            );
        public static BindableProperty ClassRoomProperty =
            BindableProperty.Create(
                propertyName: nameof(ClassRoom),
                returnType: typeof(string),
                defaultValue: string.Empty,
                declaringType: typeof(IndexCurriculumAtom),
                defaultBindingMode: BindingMode.OneWay
            );
        public static BindableProperty TeacherProperty =
            BindableProperty.Create(
                propertyName: nameof(Teacher),
                returnType: typeof(string),
                defaultValue: string.Empty,
                declaringType: typeof(IndexCurriculumAtom),
                defaultBindingMode: BindingMode.OneWay
            );
        public static BindableProperty ItemStateProperty =
            BindableProperty.Create(
                propertyName: nameof(ItemState),
                returnType: typeof(ClassState),
                declaringType: typeof(IndexCurriculumAtom),
                defaultValue: ClassState.Other,
                defaultBindingMode: BindingMode.OneWay
                );
        public static BindableProperty IsSelectedProperty =
            BindableProperty.Create(
                propertyName: nameof(IsSelected),
                returnType: typeof(bool),
                defaultValue: false,
                declaringType: typeof(IndexCurriculumAtom),
                defaultBindingMode: BindingMode.OneWay);
        public IndexCurriculumAtom()
        {
            InitializeComponent();
        }
    }
}