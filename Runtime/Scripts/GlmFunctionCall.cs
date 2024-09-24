using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlmFunctionCall : MonoBehaviour
{
    // UI组件
    [SerializeField] private InputField playerInputField;
    [SerializeField] private Text startText, destinationText, distanceText, directionText;

    public async void CreateAvatar()
    {
        // 读取玩家输入并创建用户信息
        string playerInput = playerInputField.text;
        playerInputField.text = "";
        List<SendData> chat = new List<SendData>()
        {
            new SendData("user", playerInput),
        };

        // 使用GlmFunctionTool类创建一个函数调用工具，并让其能够输出起点、终点、距离、方向的参数
        GlmFunctionTool functionTool = new GlmFunctionTool("create_avatar", "根据用户的描述，提取玩家本次的移动信息");
        functionTool.AddProperty("start", "string", "起点", false);
        functionTool.AddProperty("destination", "string", "终点", false);
        functionTool.AddProperty("distance", "int", "移动的距离（公里）", false);
        functionTool.AddProperty("direction", "string", "移动的方向", false);

        // 发送GLM函数请求，注意需要将functionTool放在一个List<GlmTool>工具列表里
        SendData response = await GlmHandler.GenerateGlmResponse(chat, 0.6f, new List<GlmTool> { functionTool });

        // GLM会根据user的输入和函数的内容，决定是使用函数工具还是生成普通对话
        // 如果response.tool_calls != null，则代表GLM使用了函数工具
        if (response.tool_calls != null) 
        {
            // 使用response.tool_calls[0].arguments_dict来获取输出参数
            Dictionary<string, string> functionOutput = response.tool_calls[0].arguments_dict;
            
            // 由于该函数的输出参数都是非强制必须包含的，所以需要检查每个参数是否被输出了
            startText.text = functionOutput.ContainsKey("start") ? functionOutput["start"] : "";
            destinationText.text = functionOutput.ContainsKey("destination") ? functionOutput["destination"] : "";
            distanceText.text = functionOutput.ContainsKey("distance") ? functionOutput["distance"] : "";
            directionText.text = functionOutput.ContainsKey("direction") ? functionOutput["direction"] : "";
        }
        // 如果user的输入与函数不相关，GLM会执行普通的对话，不调用函数。
        else
        {
            startText.text = destinationText.text = distanceText.text = directionText.text = "";
            Debug.Log(response.content);
        }
    }
}