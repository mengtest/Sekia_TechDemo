参考：https://www.findhao.net/easycoding/2480
# 1.介绍
half，也就是16 bit float point，简称fp16。
在精度可接受的情况下，使用half可提高运算速度。
本篇主要针对在shader中用half替代float的可行性进行讨论。
# 2.二进制表示
float和half都由符号位、指数部分、尾数部分构成。
float有4byte(32bit)，half有2byte(16bit)。
float   的这三部分所占的位宽分别为：1 | 8 | 23
half    的这三部分所占的位宽分别为：1 | 5 | 10
最终值 = ± 1.xxx * 2^n
    类似于科学计数法 但是计算机存储只使用二进制
符号位：0表示正数 1表示负数
指数部分：提供指数
    5个bit对应2^5=32，考虑到正负，范围为-15~16，偏移值为-15
    01111 对应十进制15 偏移后为0 2^0 = 1
    10000 对应十进制16 偏移后为1 2^1 = 2
尾数部分：表示基数的小数部分(整数部分固定为1)
    0000000000 对应1
    0010000000 对应1 + 2E-3 = 1.125
    0000000001 对应1 + 2E-10(最小的数值变化) = 1.0009765625
0的表示：0 00000 0000000000

# 大数误差大 小数误差小
我们通常希望在shader中用half表示颜色、UV、向量等，需要注意精度不足的情况。
根据数值的区间不同，最小数值变化(误差)根据2的指数倍变化。
相比float，half在尾数部分砍的太多了。
0.5~1区间       指数2^-1，误差2E-11，0.00048828125
0.25~0.5区间    指数2^-2，误差2E-12，0.000244140625
在Unity中：HALF_MIN 6.103515625e-5  // 2^-14
对于颜色数据，half通常是够得，不会因使用half产生明显的色阶误差。
对于不平铺的UV来说是够得，如果根据世界空间平铺或者跟随时间流动则误差会很大。
对于已经归一化的向量来说基本够，用于计算NdotL/NdotH之类的不会有明显误差。
    不能用half精度的向量做rsqrt这种复杂度比较高的运算，会丢失过多精度。
    两个非常相近的单位向量做dot运算时 使用half容易丢失精度？
由于我们基本要保障小数点后三位的精度，half表示的数值不应超过1。

# 在Unity shader中正确使用half
参考：https://docs.unity3d.com/Manual/SL-DataTypesAndPrecision.html
在错误的地方使用half会导致画面出现bug 在数值很大或者非常接近0时尤为注意
    如：长度小于0.01的向量归一化时 dp3指令的结果太小近似于0 rsqrt返回异常

自定义数字用half()标注 Unity的shader编译器会忽视自定义数字的h后缀
但是half(x) 和 half y = ...等转换是有效的
具体是否真的使用了half精度计算要看看性能分析结果
    float似乎不能先转换为half以后再与half数值相乘 只能以half = float * half的形式转换
    可以通过shader属性面板的编译工程生成对应平台的预览shader

half类型采样语法：
sampler2D_half _MainTex;
samplerCUBE_half _Cubemap;
可以在Player Setting中指定默认精度 默认half精度无需设置