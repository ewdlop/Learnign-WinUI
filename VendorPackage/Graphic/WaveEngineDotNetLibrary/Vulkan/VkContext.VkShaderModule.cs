using SharedLibrary.Helpers;
using System.Reflection;
using Evergine.Bindings.Vulkan;

namespace WaveEngineDotNetLibrary.Vulkan;

public unsafe partial class VkContext
{
    private VkPipelineLayout vkPipelineLayout;
    private VkPipeline vkGraphicsPipeline;

    private VkShaderModule CreateShaderModule(byte[] code)
    {
        VkShaderModuleCreateInfo createInfo = new VkShaderModuleCreateInfo
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_SHADER_MODULE_CREATE_INFO,
            pNext = null,
            flags = (uint)VkShaderModuleCreateFlags.None,
            codeSize = (UIntPtr)code.Length,
            pCode = null
        };

        fixed (byte* sourcePointer = code)
        {
            createInfo.pCode = (uint*)sourcePointer;
        }

        VkShaderModule shaderModule;
        VkHelper.CheckErrors(VulkanNative.vkCreateShaderModule(vkDevice, &createInfo, null, &shaderModule));

        return shaderModule;
    }

    private void CreateGraphicsPipeline()
    {
        Console.WriteLine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        byte[] vertShaderCode = File.ReadAllBytes($"{System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/Shaders/vert.spv");
        byte[] fragShaderCode = File.ReadAllBytes($"{System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/Shaders/frag.spv");

        VkShaderModule vertShaderModule = CreateShaderModule(vertShaderCode);
        VkShaderModule fragShaderModule = CreateShaderModule(fragShaderCode);

        VkPipelineShaderStageCreateInfo vertShaderStageInfo = new()
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_SHADER_STAGE_CREATE_INFO,
            stage = VkShaderStageFlags.VK_SHADER_STAGE_VERTEX_BIT,
            module = vertShaderModule,
            pName = "main".ToPointer(),
        };

        VkPipelineShaderStageCreateInfo fragShaderStageInfo = new()
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_SHADER_STAGE_CREATE_INFO,
            stage = VkShaderStageFlags.VK_SHADER_STAGE_FRAGMENT_BIT,
            module = fragShaderModule,
            pName = "main".ToPointer(),
        };

        VkPipelineShaderStageCreateInfo* shaderStages = stackalloc VkPipelineShaderStageCreateInfo[] 
        {  
            vertShaderStageInfo, 
            fragShaderStageInfo
        };

        // Vertex Input
        VkPipelineVertexInputStateCreateInfo vertexInputInfo = new()
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_VERTEX_INPUT_STATE_CREATE_INFO,
            vertexBindingDescriptionCount = 0,
            pVertexBindingDescriptions = null, // Optional
            vertexAttributeDescriptionCount = 0,
            pVertexAttributeDescriptions = null, // Optional
        };

        // Input assembly
        VkPipelineInputAssemblyStateCreateInfo inputAssembly = new()
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_INPUT_ASSEMBLY_STATE_CREATE_INFO,
            topology = VkPrimitiveTopology.VK_PRIMITIVE_TOPOLOGY_TRIANGLE_LIST,
            primitiveRestartEnable = false,
        };

        // Viewports and scissors
        VkViewport viewport = new()
        {
            x = 0.0f,
            y = 0.0f,
            width = vkSwapChainExtent.width,
            height = vkSwapChainExtent.height,
            minDepth = 0.0f,
            maxDepth = 1.0f,
        };

        VkRect2D scissor = new()
        {
            offset = new VkOffset2D(0, 0),
            extent = vkSwapChainExtent,
        };

        VkPipelineViewportStateCreateInfo viewportState = new()
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_VIEWPORT_STATE_CREATE_INFO,
            viewportCount = 1,
            pViewports = &viewport,
            scissorCount = 1,
            pScissors = &scissor,
        };

        // Rasterizer
        VkPipelineRasterizationStateCreateInfo rasterizer = new()
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_RASTERIZATION_STATE_CREATE_INFO,
            depthClampEnable = false,
            rasterizerDiscardEnable = false,
            polygonMode = VkPolygonMode.VK_POLYGON_MODE_FILL,
            lineWidth = 1.0f,
            cullMode = VkCullModeFlags.VK_CULL_MODE_BACK_BIT,
            frontFace = VkFrontFace.VK_FRONT_FACE_CLOCKWISE,
            depthBiasEnable = false,
            depthBiasConstantFactor = 0.0f, // Optional
            depthBiasClamp = 0.0f, // Optional
            depthBiasSlopeFactor = 0.0f, // Optional
        };

        VkPipelineMultisampleStateCreateInfo multisampling = new()
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_MULTISAMPLE_STATE_CREATE_INFO,
            sampleShadingEnable = false,
            rasterizationSamples = VkSampleCountFlags.VK_SAMPLE_COUNT_1_BIT,
            minSampleShading = 1.0f, // Optional
            pSampleMask = null, // Optional
            alphaToCoverageEnable = false, // Optional
            alphaToOneEnable = false, // Optional
        };

        // Depth and Stencil testing
        //VkPipelineDepthStencilStateCreateInfo

        // Color blending
        VkPipelineColorBlendAttachmentState colorBlendAttachment = new()
        {
            colorWriteMask = VkColorComponentFlags.VK_COLOR_COMPONENT_R_BIT |
                             VkColorComponentFlags.VK_COLOR_COMPONENT_G_BIT |
                             VkColorComponentFlags.VK_COLOR_COMPONENT_B_BIT |
                             VkColorComponentFlags.VK_COLOR_COMPONENT_A_BIT,
            blendEnable = false,
            srcColorBlendFactor = VkBlendFactor.VK_BLEND_FACTOR_ONE, // Optional
            dstColorBlendFactor = VkBlendFactor.VK_BLEND_FACTOR_ZERO, // Optional
            colorBlendOp = VkBlendOp.VK_BLEND_OP_ADD, // Optional
            srcAlphaBlendFactor = VkBlendFactor.VK_BLEND_FACTOR_ONE, // Optional
            dstAlphaBlendFactor = VkBlendFactor.VK_BLEND_FACTOR_ZERO, // Optional
            alphaBlendOp = VkBlendOp.VK_BLEND_OP_ADD, // Optional
        };

        VkPipelineColorBlendStateCreateInfo colorBlending = new()
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_COLOR_BLEND_STATE_CREATE_INFO,
            logicOpEnable = false,
            logicOp = VkLogicOp.VK_LOGIC_OP_COPY, // Optional
            attachmentCount = 1,
            pAttachments = &colorBlendAttachment,
            blendConstants_0 = 0.0f, // Optional
            blendConstants_1 = 0.0f, // Optional
            blendConstants_2 = 0.0f, // Optional
            blendConstants_3 = 0.0f, // Optional
        };

        VkPipelineLayoutCreateInfo pipelineLayoutInfo = new()
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_LAYOUT_CREATE_INFO,
            setLayoutCount = 0, // Optional
            pSetLayouts = null, // Optional
            pushConstantRangeCount = 0, // Optional
            pPushConstantRanges = null, // Optional
        };

        fixed (VkPipelineLayout* pipelineLayoutPtr = &vkPipelineLayout)
        {
            VkHelper.CheckErrors(VulkanNative.vkCreatePipelineLayout(vkDevice, &pipelineLayoutInfo, null, pipelineLayoutPtr));
        }

        VkGraphicsPipelineCreateInfo pipelineInfo = new()
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_GRAPHICS_PIPELINE_CREATE_INFO,
            stageCount = 2,
            pStages = shaderStages,
            pVertexInputState = &vertexInputInfo,
            pInputAssemblyState = &inputAssembly,
            pViewportState = &viewportState,
            pRasterizationState = &rasterizer,
            pMultisampleState = &multisampling,
            pDepthStencilState = null, // Optional
            pColorBlendState = &colorBlending,
            pDynamicState = null, // Optional
            layout = vkPipelineLayout,
            renderPass = vkRenderPass,
            subpass = 0,
            basePipelineHandle = 0, // Optional
            basePipelineIndex = -1, // Optional
        };

        fixed (VkPipeline* graphicsPipelinePtr = &vkGraphicsPipeline)
        {
            VkHelper.CheckErrors(VulkanNative.vkCreateGraphicsPipelines(vkDevice, 0, 1, &pipelineInfo, null, graphicsPipelinePtr));
        }

        VulkanNative.vkDestroyShaderModule(vkDevice, fragShaderModule, null);
        VulkanNative.vkDestroyShaderModule(vkDevice, vertShaderModule, null);
    }
}
