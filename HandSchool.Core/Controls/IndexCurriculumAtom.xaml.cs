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
        public string Description
        {
            get => GetValue(DescriptionProperty) as string;
            set => SetValue(DescriptionProperty, value);
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

        public bool IsCustom
        {
            get => (bool) GetValue(IsCustomProperty);
            set => SetValue(IsCustomProperty, value);
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
                    classRoom.Text = (IsCustom ? ClassRoom : ClassInfoSimplifier.Instance.SimplifyName(ClassRoom)).Replace('\n', ' '); break;
                case nameof(Teacher):
                    teacher.Text = Teacher; break;
                case nameof(Description):
                    desc.Text = Description; break;
                case nameof(ItemState):
                    switch (ItemState)
                    {
                        case ClassState.Other: desc.Text = ""; break;
                        case ClassState.Current: desc.Text = "现在"; break;
                        case ClassState.Next: desc.Text = "接下来"; break;
                    }
                    break;

                case nameof(IsSelected):
                    switch (Device.RuntimePlatform)
                    {
                        case Device.Android:
                            this.ScaleTo(IsSelected ? 0.97 : 0.90);
                            break;
                        default: 
                            Scale = 0.97;
                            break;
                    }
                    break;
                
                default:
                    base.OnPropertyChanged(propertyName);
                    break;
            }
        }
        public static readonly BindableProperty SectionsProperty =
            BindableProperty.Create(
                propertyName: nameof(Sections),
                returnType: typeof(string),
                defaultValue: string.Empty,
                declaringType: typeof(IndexCurriculumAtom),
                defaultBindingMode: BindingMode.OneWay
            );

        public static readonly BindableProperty DescriptionProperty =
            BindableProperty.Create(
                propertyName: nameof(Description),
                returnType: typeof(string),
                defaultValue: string.Empty,
                declaringType: typeof(IndexCurriculumAtom),
                defaultBindingMode: BindingMode.OneWay
            );
        public static readonly BindableProperty NameProperty =
            BindableProperty.Create(
                propertyName: nameof(Name),
                returnType: typeof(string),
                defaultValue: string.Empty,
                declaringType: typeof(IndexCurriculumAtom),
                defaultBindingMode: BindingMode.OneWay
            );
        public static readonly BindableProperty ClassRoomProperty =
            BindableProperty.Create(
                propertyName: nameof(ClassRoom),
                returnType: typeof(string),
                defaultValue: string.Empty,
                declaringType: typeof(IndexCurriculumAtom),
                defaultBindingMode: BindingMode.OneWay
            );
        public static readonly BindableProperty TeacherProperty =
            BindableProperty.Create(
                propertyName: nameof(Teacher),
                returnType: typeof(string),
                defaultValue: string.Empty,
                declaringType: typeof(IndexCurriculumAtom),
                defaultBindingMode: BindingMode.OneWay
            );
        public static readonly BindableProperty ItemStateProperty =
            BindableProperty.Create(
                propertyName: nameof(ItemState),
                returnType: typeof(ClassState),
                declaringType: typeof(IndexCurriculumAtom),
                defaultValue: ClassState.Other,
                defaultBindingMode: BindingMode.OneWay
                );
        public static readonly BindableProperty IsSelectedProperty =
            BindableProperty.Create(
                propertyName: nameof(IsSelected),
                returnType: typeof(bool),
                defaultValue: false,
                declaringType: typeof(IndexCurriculumAtom),
                defaultBindingMode: BindingMode.OneWay);
        
        public static readonly BindableProperty IsCustomProperty =
            BindableProperty.Create(
                propertyName: nameof(IsCustom),
                returnType: typeof(bool),
                defaultValue: false,
                declaringType: typeof(IndexCurriculumAtom),
                defaultBindingMode: BindingMode.OneWay);
        public IndexCurriculumAtom()
        {
            InitializeComponent();
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    Scale = 0.90;
                    break;
                default: Scale = 0.97;
                    break;
            }
        }
    }
}