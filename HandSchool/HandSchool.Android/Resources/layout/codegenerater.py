for i in range(1,13):
    f=open(f"singleclassitem_{i}.axml","w",encoding="utf-8")
    a=f"""<?xml version="1.0" encoding="utf-8"?>
    <!--批量生成代码，自己修改累坏警告-->
<LinearLayout
 xmlns:android="http://schemas.android.com/apk/res/android"
    android:layout_width="fill_parent"
    android:layout_height="0dip"
    android:padding="1dip"
    android:layout_weight="{i}"
>
    <TextView
        android:id="@+id/class{i}"
        android:gravity="center"
        android:padding="1dip"
        android:layout_width="fill_parent"
        android:layout_height="fill_parent"
        android:textSize="9sp" />
</LinearLayout>
    """
    f.write(a)
    f.close()