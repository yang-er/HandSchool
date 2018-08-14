# iOS版说明

## 发行说明

由于没有开发者人员账户，并且由于功能原因，可能无法通过AppStore审核。
可能会考虑去购买应用代上架服务，但是审核仍然是一个问题。

- 使用的教务系统没有提供IPv6的服务器访问。
- 提供了部分脚本代码热更新的功能。
- 提到了本app拥有Android和Windows 10的版本。

## 安装方法

### 安装环境

为了安装本app，您可能需要

- 一台运行新版 macOS 的电脑
- Xcode 新版
- Visual Studio for Mac 新版
- （以上不能通过Windows的VMware配置）

或者

- 一个拥有上述环境的土豪朋友

在您确定上述环境齐备时，可以开始安装啦！

### 从源代码编译

首先需要打开Xcode，申请一个iOS Development Certificate。
然后打开macOS的 `终端` App，运行下述代码
```bash
cd ~/Desktop
git clone https://github.com/yang-er/HandSchool.git
```
这时，您的桌面上将出现一个名为`HandSchool`的文件夹。
使用`Visual Studio for Mac`打开`HandSchool.sln`。
加载时，对于`HandSchool.Android`和`HandSchool.UWP`，右键`卸载项目`。
对于`HandSchool.iOS`，右键`设为启动项目`，右键`属性`，找到`iOS 清单`项，将`Bundle Identify`的前缀`tlylz`修改为其他字符串，可以是您的姓名。
将iPhone/iPad连接到电脑，选择编译目标，将调试设备选为您的设备，然后按下左上角的运行按钮。
这时候，程序应该已经可以在您的电脑上运行了。
切换到`终端`，然后运行
```bash
git stash
```
以后需要更新的时候，可以直接运行
```bash
git pull
```
就可以更新app源码了，之后可以再次运行`Visual Studio for Mac`编译程序。

## 说明
本说明更新于2018-8-14，后期将不定期更新。
