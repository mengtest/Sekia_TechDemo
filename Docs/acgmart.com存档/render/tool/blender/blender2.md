---
title: Blender雕刻
categories:
- [渲染, 工具, Blender]
date: 2018-11-24 10:10:47
---

\[toc\]本篇中讨论Blender中雕刻工具的用法

# 雕刻模式外部分快捷键

细分单个物体：在物体模式下-Ctrl+4-应用修改器 应用：Ctrl+A-应用旋转&缩放 设置3D游标位置：Shift+右键 复制物体：Shift+D-回车-R+Z-90-回车

# 雕刻模式下快捷键

切换模式：Ctrl+Tab 切换视角：~ 切换显示模式：Z 切换笔刷：Space 最大化：Ctrl+Alt+Space 个人喜好：Q 开启动态拓扑：Ctrl+D-确定 修改笔刷强度：Shift+F 修改动态拓扑细节大小：Shift+D-调整雕刻时的最大边长/输入数字-左键确定 （指定） 负强度笔刷：按住Ctrl+使用笔刷 平滑（S）：按住Shift+使用笔刷 标记并挤出：使用Mask笔刷刷黑-Ctrl+I-使用蛇钩笔刷挤出

# 名词解释

半径：笔刷的半径，单位为像素，影响操作的精度，可以使用大号的笔刷快速制作初始形态，然后缩小笔刷修改细节。 强度/力度：对物体修改结果完成度，0.5强度时画出的线高度和宽度均为1强度时的一半，可以多刷几次实现同样效果。 **动态拓扑**：未开启动态拓扑时使用笔刷不会新增点/线/面，可以调整模型的形状但是面数不够时无法进一步勾勒细节；开启动态拓扑后使用笔刷会增加/减少面数，拉远与物体的距离使用笔刷时删除细节并减少面数，拉近时增加细节并增加面数 关于动态拓扑的开启时机：动态拓扑可以用来增加细节，在细节不足时开启并使用可以改变细节的笔刷轻按即可修改细节，关闭动态拓扑后放大物体修改细节。 细节大小：在未开启动态拓扑时，笔刷将所经过的点根据笔刷外形重新排列，细节大小由笔刷经过的点之间的密度决定；开启动态拓扑后打破了固定面数的限制，笔刷所经过的区域将使用指定密度且数量充足的点根据笔刷的外形重新排列，这里指定的密度也就是细节大小，单位为像素。 切换笔刷曲线：选择笔刷后使用笔刷可以得到默认效果，在上方快捷栏的曲线中可以选择笔刷的曲线。对于增量笔刷来说，笔刷曲线相当于笔刷划过路径的截面图，笔刷曲线的视图则是截面的右半效果。 G和K笔刷的区别：在调整轮廓时都可以使用，开启动态拓扑后K笔刷可以补充细节并制造条状凸起，K的使用方法比G更灵活。 创建对称采样点：在开始雕刻时创建一个空物体，保留该物体用做对称采样目标。

# 笔刷

## 增量笔刷

▲ X：Draw：画笔：表现为在指定位置的法线方向创造凸起；可以连续点击某个位置创造凸起/凹陷效果，滑动画出一条线/坑。 △ C：Clay：泥土：表现为在指定位置的法线方向创造微微隆起；可以连续点击某个位置创造小土丘效果，土丘高处平滑。 △ 1：Clay Strips：表现为在滑动路径上创造平整的隆起；单击没有作用，笔刷路径两侧有明显折痕。 △ L：Layer：表现为在在指定位置的法线方向创造拉扯性凸起；**开启动态拓扑后不增加面数**。 △ I：Inflate：膨胀：表现为在指定位置的法线方向创造半球形凸起；在同一个区域滑动可以画出的球形凸起。 △ 2：Blob：表现为在指定区域的法线方向创造半球形凸起；对笔刷边缘的面的角度修正低，可造成连续山丘效果。 ▲ Shift+C：Crease：表现为在指定区域的法线方向创造尖锐凹槽；凹槽顶端汇聚于一点，可造成明显折痕效果。

## 修整笔刷

▲ S：Smooth：平滑：表现为使指定区域内的面的角度接近笔刷边缘的面的角度；**开启动态拓扑后不增加面数**；用于消除细节。 △ Shift+T：Flatten：表现为使区域内面的角度接近平均角度；会因为吸纳笔刷边缘的凸起和凹槽而均和角度；用于消除细节。 △ 3：Fill：填坑：表现为使区域中间平均高度低于周边的面创造凸起，实现填充凹槽的效果。 ▲ 4：Scrape：打磨：表现为去除区域中间的凸起效果。 △ P：Pinch：勒缝：表现为使已有的不平整表面的点向中间聚合实现线形微微凸起效果；中间有淡淡折痕。

## 塑形笔刷

▲ G：Grab：拉扯：表现为平移所选区域顶点位置；向外拉扯时，对中间区域顶点的效果更明显；**开启动态拓扑后不增加面数**。 △ K：Snake Hook：蛇形钩：对指定区域造成拉扯效果，拉扯的过程中能自动补充细节，可画出角/头发/耳朵等突出部位。 △ 5：Thumb：表现为所选区域顶点向笔刷滑动方向集中；**开启动态拓扑后不增加面数**。 △ 6：Nudge：轻推：表现为所选区域顶点向笔刷滑动方向少量位移；**开启动态拓扑后不增加面数**。 △ 7：Rotate：旋转：在所选区域有足够的面时尝试使面顺/逆时针旋转，形成不规则凸起。

## 遮罩笔刷

▲ 8：Mask：遮罩：被遮罩笔刷染成全黑的区域不会被其他笔刷修改 △ 9：Simplify： △ Shift+H：Box Hide：被框选的区域顶点被隐藏且不可被编辑 △ B：Box Mask：被框选区域的顶点变黑且不可被编辑

# 雕刻流程

## 绘制参考图

创建GP单色图：在物体模式下，Shift+A-Grease Pencil-单色图，切换到绘制模式； 设置上方的Stroke Placement，默认为原点。修改为面后，对模型画线时线会贴在模型上。 在绘制模式，物体数据的图标变成笔形，下笔后自动创建图层； 修改图层显示深度：物体数据-Viewport Display-Depth Ordering-前；

## 绘制参考线

在物体模式下-Space+D选择Annotate备注工具-工具设置中设置置换为“面”-绘制参考线-切换为雕刻模式-Overlays中勾选Annotate 参考线：在右视图中绘制侧面轮廓，包括：发际线/脸轮廓/眼窝/鼻子/嘴唇/下巴/耳朵。另外绘制一个45°角轮廓便于把握侧面厚度。 管理参考线：N-Annotate-新建/删除备注-显示/隐藏备注

## 插入背景图片

创建图片：在物体模式下，Shift+A-图像-Background-选择图像-调整位置/旋转/大小-修改物体数据：透明度

## 绘制遮罩区域

雕刻模式下-按住Ctrl+Shift-鼠标圈选出一个区域：变黑的区域将被遮罩-M（切换为遮罩笔刷）-调整遮罩区域周边细节-Ctrl+I反转-使用低强度的G调整细节 取消遮罩：Alt+M 在绘制双唇时，先做好上嘴唇，给上嘴唇涂上遮罩后再做下嘴唇。

## 设置焦距

焦距的大小决定了雕刻细度，有一个固定的焦距方便把控细节程度。 设置焦距：N-视图-焦距-调整为150mm

## 设置着色方式

雕刻界面Sculpting-着色方式-勾选“轮廓”

## 对称左右

在需要有对称效果时，开启动态拓扑-方向-选择“+X到-X”-对称

## 增加其他部件

快速捏出初始形态后，就要开始雕刻模型的细节了。 部分器官适合在父级上雕刻出来，如鼻子/嘴巴，在原型基础上创造凸起和凹槽即可。 有一定细节/**对父级外形依赖度**不高的部位可以在外部创建好后与父级合并，如耳朵/眼球。 对父级外形依赖度高的部位，如头发/外套，可以创建物体后沿着父级捏以获得初始形态。