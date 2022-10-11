苹果官方文档：https://developer.apple.com/documentation/metal/
    主要看开发工具下的调试工具和性能调优
部分文档有中文版：https://developer.apple.com/cn/documentation/
由于我主要是Unity的使用者 学习Metal主要是将知识点和Unity进行对接

# Metal(MTL)文档
MSL：Metal Shading Language，与Unity shader大同小异
device address space：GPU可以读和写的持久内存
MTLDevice：对GPU设备的抽象
GPU-related entities：shaders、memoryBuffers、textures
MTLFunction：Metal Function，public的，无法被其他shader方法调用，作为接口
MTLLibrary：Metal Function的集合
MTLComputePipelineState(PSO)：Metal Function的状态集合
    Metal Function不能直接执行 需要通过创建一个pipeline转换为可执行代码
    pipeline指定了GPU完成特定任务的多个步骤 通过PSO的形式表现出来
    最简单的 compute pipeline只包含一个Metal Function，GPU传回数据给CPU
    创建PSO时，以同步的方式针对GPU编译shader，应注意耗时。
MTLBuffer：类似于ComputeBuffer，需要声明并指定长度，填充数据，绑定PSO
    MTLResourceOptions：可指定Buffer的的保存形式 比如CPU/GPU访问权和读写权
MTLCommandQueue/MTLCommandBuffer/MTLComputeCommandEncoder：规划渲染命令列表
    添加compute pass时使用MTLComputeCommandEncoder
    光栅shader使用MTLRenderCommandEncoder
MTKView：类似于SRP，对渲染流程进行包装
    enableSetNeedsDisplay：是否为静帧，如果是则只在view变化时update画面
    delegate：类似于URP的Renderer，当需要update画面时或分辨率变化时调用
    currentDrawable：backBUffer
MTLRenderPassDescriptor：描述渲染目标(RT) load和store行为
renderEncoder drawPrimitives：相当于Unity中的cmd.DrawMesh
MTLRenderPipelineDescriptor：描述光栅shader的一次DrawCall 用于创建PSO

# Captured GPU Workload
截帧成功后得到的调试信息，相当于RenderDoc的.rdc文件
右侧有最终渲染画面 鼠标放在像素上可显示颜色值
# Summary
Export：导出为.gputrace文件
Overview：性能消耗总览
    Draw Calls：DC数 核心指标
Performance：性能
    GPU Time：GPU侧的浮点运算延迟 核心指标
    Vertices：顶点数
Memory：内存
    Textures：纹理
    Buffers：
    Other：
# 查看DrawCall
通过添加lable来管理当前关注的渲染任务 这类似于Unity的CommandBuffer性能打点
点击drawPrimitives任务查看光栅化后的几何体 这类似于RenderDoc的MeshViewer
# Geometry Viewer和Shader Debugger
在DrawCall信息中点击Geometry项查看 可查看顶点信息
选中一个顶点后 点击Debug可调试顶点shader
    这个功能是Unity里没有的 类似于C#的逐行调试
    可以用于判断错误来源于输入数据还是shader代码
    展开调试信息后获得更多调试信息
    可观察一个变量的数值的变化过程
选中一个像素后 点击Debug按钮可调试片元shader
# Memory Viewer
寻找资源分配中可以优化的点
易失性/非易失性 私有的/共享的 未使用的 未绑定的 绑定了但是无GPU访问的
# 帧性能分析
渲染命令支持有两种排序方式 在旧的XCode版本排序方式名称有差别
在Group by API Call模式下检查管线流程和渲染逻辑
在Group by Pipeline State模式下根据GPU Time排序DrawCall
    DrawCall、方法、代码行 都会有性能分析帮助定位性能热点
    以代码行为优化着手点-代码行的性能分析饼图提供性能指标
        ALU：逻辑计算耗时
            使用half精度或避免使用复杂指令 节约ALU
        Memory：buffer或texture的访问延迟
            可通过改变分辨率调整延迟
        Control flow：逻辑分支耗时
        Synchronization：同步延迟 异步计算中需要等待同步
# 在真机非调试环境生成截帧数据
    Capture GPU Traces Without Xcode
    但是输出的文件需要匹配的真机运行？
https://developer.apple.com/videos/play/wwdc2019/606/

https://developer.apple.com/documentation/metal/debugging_tools/improving_memory_and_resource_usage