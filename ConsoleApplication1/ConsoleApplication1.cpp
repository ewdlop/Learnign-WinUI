// ===== INCLUDES =====
#include <GL/glew.h>
#include <GLFW/glfw3.h>
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>
#include <imgui.h>
#include <imgui_impl_glfw.h>
#include <imgui_impl_opengl3.h>
#include <iostream>
#include <vector>
#include <fstream>
#include <sstream>
#include <cmath>
#include <stack>
#include <memory>
#include <algorithm>

#ifndef M_PI  
#define M_PI 3.14159265358979323846  
#endif  

// ===== TYPES AND ENUMS =====
enum class ObjectType {
    CUBE,
    SPHERE,
    CYLINDER,
    CONE,
    PLANE
};

// ===== FORWARD DECLARATIONS =====
class Object3D;
class Command;
class CommandHistory;
class Camera;
class Grid;
class CADApplication;

// ===== 3D OBJECT CLASS =====
class Object3D {
public:
    std::vector<float> vertices;
    std::vector<unsigned int> indices;
    glm::mat4 modelMatrix;
    ObjectType type;
    glm::vec3 color;
    bool isSelected;
    std::string name;

    Object3D(ObjectType t, const std::string& n) :
        modelMatrix(glm::mat4(1.0f)),
        type(t),
        color(glm::vec3(0.5f, 0.5f, 0.5f)),
        isSelected(false),
        name(n) {
    }

    void translate(glm::vec3 translation) {
        modelMatrix = glm::translate(modelMatrix, translation);
    }

    void rotate(float angle, glm::vec3 axis) {
        modelMatrix = glm::rotate(modelMatrix, angle, axis);
    }

    void scale(glm::vec3 scale) {
        modelMatrix = glm::scale(modelMatrix, scale);
    }
};


// ===== COMMAND PATTERN CLASSES =====
class Command {
public:
    virtual ~Command() {}
    virtual void execute() = 0;
    virtual void undo() = 0;
};

class CreateObjectCommand : public Command {
private:
    std::vector<Object3D*>& objects;
    Object3D* object;

public:
    CreateObjectCommand(std::vector<Object3D*>& objs, Object3D* obj)
        : objects(objs), object(obj) {
    }

    void execute() override {
        objects.push_back(object);
    }

    void undo() override {
        auto it = std::find(objects.begin(), objects.end(), object);
        if (it != objects.end()) {
            objects.erase(it);
        }
    }
};

class DeleteObjectCommand : public Command {
private:
    std::vector<Object3D*>& objects;
    Object3D* object;
    size_t previousIndex;

public:
    DeleteObjectCommand(std::vector<Object3D*>& objs, Object3D* obj)
        : objects(objs), object(obj) {
        auto it = std::find(objects.begin(), objects.end(), object);
        if (it != objects.end()) {
            previousIndex = std::distance(objects.begin(), it);
        }
    }

    void execute() override {
        auto it = std::find(objects.begin(), objects.end(), object);
        if (it != objects.end()) {
            objects.erase(it);
        }
    }

    void undo() override {
        objects.insert(objects.begin() + previousIndex, object);
    }
};

class TransformCommand : public Command {
private:
    Object3D* object;
    glm::mat4 oldTransform;
    glm::mat4 newTransform;

public:
    TransformCommand(Object3D* obj, const glm::mat4& oldT, const glm::mat4& newT)
        : object(obj), oldTransform(oldT), newTransform(newT) {
    }

    void execute() override {
        object->modelMatrix = newTransform;
    }

    void undo() override {
        object->modelMatrix = oldTransform;
    }
};

class CommandHistory {
private:
    std::stack<std::unique_ptr<Command>> undoStack;
    std::stack<std::unique_ptr<Command>> redoStack;

public:
    void executeCommand(std::unique_ptr<Command> command) {
        command->execute();
        undoStack.push(std::move(command));
        while (!redoStack.empty()) {
            redoStack.pop();
        }
    }

    void undo() {
        if (!undoStack.empty()) {
            std::unique_ptr<Command> command = std::move(undoStack.top());
            undoStack.pop();
            command->undo();
            redoStack.push(std::move(command));
        }
    }

    void redo() {
        if (!redoStack.empty()) {
            std::unique_ptr<Command> command = std::move(redoStack.top());
            redoStack.pop();
            command->execute();
            undoStack.push(std::move(command));
        }
    }

    bool canUndo() const { return !undoStack.empty(); }
    bool canRedo() const { return !redoStack.empty(); }
};

// ===== CAMERA CLASS =====
class Camera {
private:
    glm::vec3 position;
    glm::vec3 front;
    glm::vec3 up;
    glm::vec3 right;
    glm::vec3 worldUp;
    float yaw;
    float pitch;
    float movementSpeed;
    float mouseSensitivity;
    float zoomLevel;

    void updateCameraVectors() {
        glm::vec3 front;
        front.x = cos(glm::radians(yaw)) * cos(glm::radians(pitch));
        front.y = sin(glm::radians(pitch));
        front.z = sin(glm::radians(yaw)) * cos(glm::radians(pitch));
        this->front = glm::normalize(front);
        right = glm::normalize(glm::cross(this->front, worldUp));
        up = glm::normalize(glm::cross(right, this->front));
    }

public:
    Camera(glm::vec3 pos = glm::vec3(0.0f, 0.0f, 5.0f)) :
        position(pos),
        front(glm::vec3(0.0f, 0.0f, -1.0f)),
        worldUp(glm::vec3(0.0f, 1.0f, 0.0f)),
        yaw(-90.0f),
        pitch(0.0f),
        movementSpeed(5.0f),
        mouseSensitivity(0.1f),
        zoomLevel(45.0f)
    {
        updateCameraVectors();
    }

    glm::mat4 getViewMatrix() {
        return glm::lookAt(position, position + front, up);
    }

    float getZoom() { return zoomLevel; }

    void processKeyboard(char direction, float deltaTime) {
        float velocity = movementSpeed * deltaTime;
        if (direction == 'W') position += front * velocity;
        if (direction == 'S') position -= front * velocity;
        if (direction == 'A') position -= right * velocity;
        if (direction == 'D') position += right * velocity;
        if (direction == 'Q') position += up * velocity;
        if (direction == 'E') position -= up * velocity;
    }

    void processMouse(float xoffset, float yoffset) {
        xoffset *= mouseSensitivity;
        yoffset *= mouseSensitivity;

        yaw += xoffset;
        pitch += yoffset;

        if (pitch > 89.0f) pitch = 89.0f;
        if (pitch < -89.0f) pitch = -89.0f;

        updateCameraVectors();
    }

    void processScroll(float yoffset) {
        zoomLevel -= yoffset;
        if (zoomLevel < 1.0f) zoomLevel = 1.0f;
        if (zoomLevel > 90.0f) zoomLevel = 90.0f;
    }
};

// ===== GRID HELPER CLASS =====
class Grid {
public:
    std::vector<float> vertices;
    GLuint VAO, VBO;

    Grid(float size = 10.0f, float spacing = 1.0f) {
        createGrid(size, spacing);
        setupBuffers();
    }

    void createGrid(float size, float spacing) {
        vertices.clear();

        for (float i = -size; i <= size; i += spacing) {
            vertices.push_back(i);
            vertices.push_back(0.0f);
            vertices.push_back(-size);
            vertices.push_back(i);
            vertices.push_back(0.0f);
            vertices.push_back(size);

            vertices.push_back(-size);
            vertices.push_back(0.0f);
            vertices.push_back(i);
            vertices.push_back(size);
            vertices.push_back(0.0f);
            vertices.push_back(i);
        }
    }

    void setupBuffers() {
        glGenVertexArrays(1, &VAO);
        glGenBuffers(1, &VBO);

        glBindVertexArray(VAO);
        glBindBuffer(GL_ARRAY_BUFFER, VBO);
        glBufferData(GL_ARRAY_BUFFER, vertices.size() * sizeof(float), &vertices[0], GL_STATIC_DRAW);

        glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 3 * sizeof(float), (void*)0);
        glEnableVertexAttribArray(0);

        glBindVertexArray(0);
    }

    void draw() {
        glBindVertexArray(VAO);
        glDrawArrays(GL_LINES, 0, vertices.size() / 3);
        glBindVertexArray(0);
    }
};

// ===== MAIN APPLICATION CLASS =====
class CADApplication {
private:
    // ===== MEMBER VARIABLES =====
    GLFWwindow* window;
    Camera camera;
    std::vector<Object3D*> objects;
    Object3D* selectedObject;
    Grid grid;
    bool firstMouse;
    float lastX, lastY;
    float deltaTime, lastFrame;

    // Shader programs
    GLuint shaderProgram;
    GLuint gridShaderProgram;

    // UI state
    int selectedObjectIndex;
    char nameBuffer[64];

    // Measurement tool
    bool measurementMode;
    glm::vec3 measurementStart;
    glm::vec3 measurementEnd;
    bool measurementStartSet;

    // Command history
    CommandHistory commandHistory;
    glm::mat4 oldTransform;

public:
    // ===== CONSTRUCTOR/DESTRUCTOR =====
    CADApplication() : firstMouse(true), lastX(800.0f / 2.0f), lastY(600.0f / 2.0f),
        deltaTime(0.0f), lastFrame(0.0f), shaderProgram(0),
        selectedObject(nullptr), grid(Grid()), selectedObjectIndex(-1),
        measurementMode(false), measurementStartSet(false) {
        memset(nameBuffer, 0, sizeof(nameBuffer));
    }

    // ===== INITIALIZATION =====
    bool initialize() {
        if (!glfwInit()) {
            std::cerr << "Failed to initialize GLFW" << std::endl;
            return false;
        }

        window = glfwCreateWindow(1200, 800, "Advanced 3D CAD Software", NULL, NULL);
        if (!window) {
            std::cerr << "Failed to create GLFW window" << std::endl;
            glfwTerminate();
            return false;
        }

        glfwMakeContextCurrent(window);

        if (glewInit() != GLEW_OK) {
            std::cerr << "Failed to initialize GLEW" << std::endl;
            return false;
        }

        // Set callbacks
        glfwSetCursorPosCallback(window, [](GLFWwindow* w, double x, double y) {
            auto app = static_cast<CADApplication*>(glfwGetWindowUserPointer(w));
            if (app) app->mouseCallback(x, y);
            });

        glfwSetScrollCallback(window, [](GLFWwindow* w, double x, double y) {
            auto app = static_cast<CADApplication*>(glfwGetWindowUserPointer(w));
            if (app) app->scrollCallback(y);
            });

        glfwSetMouseButtonCallback(window, [](GLFWwindow* w, int button, int action, int mods) {
            auto app = static_cast<CADApplication*>(glfwGetWindowUserPointer(w));
            if (app) app->mouseButtonCallback(button, action, mods);
            });

        glfwSetWindowUserPointer(window, this);

        glEnable(GL_DEPTH_TEST);

        IMGUI_CHECKVERSION();
        ImGui::CreateContext();
        ImGui_ImplGlfw_InitForOpenGL(window, true);
        ImGui_ImplOpenGL3_Init("#version 330");

        shaderProgram = createShaderProgram();
        gridShaderProgram = createGridShaderProgram();

        return true;
    }

    // ===== MAIN LOOP =====
    void run() {
        while (!glfwWindowShouldClose(window)) {
            float currentFrame = glfwGetTime();
            deltaTime = currentFrame - lastFrame;
            lastFrame = currentFrame;

            processInput();

            glClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

            renderGrid();
            renderObjects();
            renderUI();

            glfwSwapBuffers(window);
            glfwPollEvents();
        }
    }

    // ===== CLEANUP =====
    void cleanup() {
        for (auto obj : objects) {
            delete obj;
        }

        ImGui_ImplOpenGL3_Shutdown();
        ImGui_ImplGlfw_Shutdown();
        ImGui::DestroyContext();

        glDeleteProgram(shaderProgram);
        glDeleteProgram(gridShaderProgram);
        glfwDestroyWindow(window);
        glfwTerminate();
    }

private:
    // ===== OBJECT CREATION METHODS =====
    void createCube(const std::string& name = "Cube") {
        Object3D* cube = new Object3D(ObjectType::CUBE, name);

        cube->vertices = {
            -0.5f, -0.5f,  0.5f,  0.5f, -0.5f,  0.5f,  0.5f,  0.5f,  0.5f, -0.5f,  0.5f,  0.5f,
            -0.5f, -0.5f, -0.5f,  0.5f, -0.5f, -0.5f,  0.5f,  0.5f, -0.5f, -0.5f,  0.5f, -0.5f
        };

        cube->indices = {
            0, 1, 2, 2, 3, 0,   1, 5, 6, 6, 2, 1,   5, 4, 7, 7, 6, 5,
            4, 0, 3, 3, 7, 4,   3, 2, 6, 6, 7, 3,   4, 5, 1, 1, 0, 4
        };

        auto command = std::make_unique<CreateObjectCommand>(objects, cube);
        commandHistory.executeCommand(std::move(command));
    }

    void createSphere(const std::string& name = "Sphere", int segments = 32) {
        Object3D* sphere = new Object3D(ObjectType::SPHERE, name);

        for (int lat = 0; lat <= segments; lat++) {


            // Replace the problematic line with the following:  
            float theta = lat * static_cast<float>(M_PI) / segments;
            float sinTheta = sin(theta);
            float cosTheta = cos(theta);

            for (int lon = 0; lon <= segments; lon++) {
                float phi = lon * 2 * M_PI / segments;
                float sinPhi = sin(phi);
                float cosPhi = cos(phi);

                float x = cosPhi * sinTheta;
                float y = cosTheta;
                float z = sinPhi * sinTheta;

                sphere->vertices.push_back(x);
                sphere->vertices.push_back(y);
                sphere->vertices.push_back(z);
            }
        }

        for (int lat = 0; lat < segments; lat++) {
            for (int lon = 0; lon < segments; lon++) {
                int first = lat * (segments + 1) + lon;
                int second = first + segments + 1;

                sphere->indices.push_back(first);
                sphere->indices.push_back(second);
                sphere->indices.push_back(first + 1);

                sphere->indices.push_back(second);
                sphere->indices.push_back(second + 1);
                sphere->indices.push_back(first + 1);
            }
        }

        auto command = std::make_unique<CreateObjectCommand>(objects, sphere);
        commandHistory.executeCommand(std::move(command));
    }

    void createCylinder(const std::string& name = "Cylinder", int segments = 32) {
        Object3D* cylinder = new Object3D(ObjectType::CYLINDER, name);

        for (int i = 0; i <= segments; i++) {
            float theta = i * 2.0f * M_PI / segments;
            float x = cos(theta);
            float z = sin(theta);

            cylinder->vertices.push_back(x);
            cylinder->vertices.push_back(-0.5f);
            cylinder->vertices.push_back(z);

            cylinder->vertices.push_back(x);
            cylinder->vertices.push_back(0.5f);
            cylinder->vertices.push_back(z);
        }

        for (int i = 0; i < segments; i++) {
            int current = i * 2;
            int next = ((i + 1) % segments) * 2;

            cylinder->indices.push_back(current);
            cylinder->indices.push_back(next);
            cylinder->indices.push_back(current + 1);

            cylinder->indices.push_back(next);
            cylinder->indices.push_back(next + 1);
            cylinder->indices.push_back(current + 1);
        }

        auto command = std::make_unique<CreateObjectCommand>(objects, cylinder);
        commandHistory.executeCommand(std::move(command));
    }

    void createPlane(const std::string& name = "Plane") {
        Object3D* plane = new Object3D(ObjectType::PLANE, name);

        plane->vertices = {
            -1.0f, 0.0f, -1.0f,  1.0f, 0.0f, -1.0f,  1.0f, 0.0f,  1.0f, -1.0f, 0.0f,  1.0f
        };

        plane->indices = {
            0, 1, 2,  2, 3, 0
        };

        auto command = std::make_unique<CreateObjectCommand>(objects, plane);
        commandHistory.executeCommand(std::move(command));
    }

    // ===== INPUT HANDLING =====
    void processInput() {
        if (glfwGetKey(window, GLFW_KEY_ESCAPE) == GLFW_PRESS)
            glfwSetWindowShouldClose(window, true);

        if (glfwGetKey(window, GLFW_KEY_W) == GLFW_PRESS)
            camera.processKeyboard('W', deltaTime);
        if (glfwGetKey(window, GLFW_KEY_S) == GLFW_PRESS)
            camera.processKeyboard('S', deltaTime);
        if (glfwGetKey(window, GLFW_KEY_A) == GLFW_PRESS)
            camera.processKeyboard('A', deltaTime);
        if (glfwGetKey(window, GLFW_KEY_D) == GLFW_PRESS)
            camera.processKeyboard('D', deltaTime);
        if (glfwGetKey(window, GLFW_KEY_Q) == GLFW_PRESS)
            camera.processKeyboard('Q', deltaTime);
        if (glfwGetKey(window, GLFW_KEY_E) == GLFW_PRESS)
            camera.processKeyboard('E', deltaTime);

        if (glfwGetKey(window, GLFW_KEY_LEFT_CONTROL) == GLFW_PRESS && glfwGetKey(window, GLFW_KEY_Z) == GLFW_PRESS)
            commandHistory.undo();
        if (glfwGetKey(window, GLFW_KEY_LEFT_CONTROL) == GLFW_PRESS && glfwGetKey(window, GLFW_KEY_Y) == GLFW_PRESS)
            commandHistory.redo();
    }

    void mouseCallback(double xpos, double ypos) {
        if (firstMouse) {
            lastX = xpos;
            lastY = ypos;
            firstMouse = false;
        }

        float xoffset = xpos - lastX;
        float yoffset = lastY - ypos;
        lastX = xpos;
        lastY = ypos;

        if (glfwGetMouseButton(window, GLFW_MOUSE_BUTTON_LEFT) == GLFW_PRESS)
            camera.processMouse(xoffset, yoffset);
    }

    void scrollCallback(double yoffset) {
        camera.processScroll(yoffset);
    }

    void mouseButtonCallback(int button, int action, int mods) {
        if (button == GLFW_MOUSE_BUTTON_LEFT && action == GLFW_PRESS) {
            if (measurementMode) {
                double xpos, ypos;
                glfwGetCursorPos(window, &xpos, &ypos);

                glm::vec3 point = screenToWorld(xpos, ypos);

                if (!measurementStartSet) {
                    measurementStart = point;
                    measurementStartSet = true;
                }
                else {
                    measurementEnd = point;
                    measurementMode = false;
                    measurementStartSet = false;
                }
            }
            else {
                performObjectSelection();
            }
        }
    }

    glm::vec3 screenToWorld(double xpos, double ypos) {
        float x = (2.0f * xpos) / 1200 - 1.0f;
        float y = 1.0f - (2.0f * ypos) / 800;

        glm::vec4 clip = glm::vec4(x, y, -1.0f, 1.0f);

        glm::mat4 projection = glm::perspective(glm::radians(camera.getZoom()), 1200.0f / 800.0f, 0.1f, 100.0f);
        glm::vec4 eye = glm::inverse(projection) * clip;
        eye = glm::vec4(eye.x, eye.y, -1.0f, 0.0f);

        glm::mat4 view = camera.getViewMatrix();
        glm::vec3 world = glm::normalize(glm::vec3(glm::inverse(view) * eye));

        glm::vec3 cameraPos = glm::vec3(0.0f, 0.0f, 5.0f);
        float t = -cameraPos.y / world.y;
        return cameraPos + t * world;
    }

    void performObjectSelection() {
        // Ray casting selection logic would go here
    }

    // ===== FILE OPERATIONS =====
    void saveToOBJ(const std::string& filename) {
        std::ofstream file(filename);
        if (!file.is_open()) {
            std::cerr << "Failed to open file for writing: " << filename << std::endl;
            return;
        }

        file << "# 3D CAD Export\n";

        int vertexOffset = 0;
        for (const auto& obj : objects) {
            file << "o " << obj->name << "\n";

            for (size_t i = 0; i < obj->vertices.size(); i += 3) {
                glm::vec3 vertex(obj->vertices[i], obj->vertices[i + 1], obj->vertices[i + 2]);
                glm::vec4 transformed = obj->modelMatrix * glm::vec4(vertex, 1.0f);

                file << "v " << transformed.x << " " << transformed.y << " " << transformed.z << "\n";
            }

            for (size_t i = 0; i < obj->indices.size(); i += 3) {
                file << "f " << (obj->indices[i] + vertexOffset + 1) << " "
                    << (obj->indices[i + 1] + vertexOffset + 1) << " "
                    << (obj->indices[i + 2] + vertexOffset + 1) << "\n";
            }

            vertexOffset += obj->vertices.size() / 3;
        }

        file.close();
        std::cout << "Scene saved to " << filename << std::endl;
    }

    void loadFromOBJ(const std::string& filename) {
        std::ifstream file(filename);
        if (!file.is_open()) {
            std::cerr << "Failed to open file for reading: " << filename << std::endl;
            return;
        }

        for (auto obj : objects) {
            delete obj;
        }
        objects.clear();

        std::vector<glm::vec3> tempVertices;
        Object3D* currentObject = nullptr;
        std::string line;

        while (std::getline(file, line)) {
            std::istringstream iss(line);
            std::string prefix;
            iss >> prefix;

            if (prefix == "o") {
                if (currentObject) {
                    objects.push_back(currentObject);
                }
                std::string name;
                iss >> name;
                currentObject = new Object3D(ObjectType::CUBE, name);
            }
            else if (prefix == "v") {
                float x, y, z;
                iss >> x >> y >> z;
                tempVertices.push_back(glm::vec3(x, y, z));
            }
            else if (prefix == "f" && currentObject) {
                unsigned int indices[3];
                iss >> indices[0] >> indices[1] >> indices[2];

                for (int i = 0; i < 3; i++) {
                    indices[i]--;
                    currentObject->indices.push_back(indices[i]);

                    glm::vec3 vertex = tempVertices[indices[i]];
                    currentObject->vertices.push_back(vertex.x);
                    currentObject->vertices.push_back(vertex.y);
                    currentObject->vertices.push_back(vertex.z);
                }
            }
        }

        if (currentObject) {
            objects.push_back(currentObject);
        }

        file.close();
        std::cout << "Scene loaded from " << filename << std::endl;
    }

    // ===== RENDERING =====
    void renderGrid() {
        glUseProgram(gridShaderProgram);

        glm::mat4 projection = glm::perspective(glm::radians(camera.getZoom()), 1200.0f / 800.0f, 0.1f, 100.0f);
        glm::mat4 view = camera.getViewMatrix();
        glm::mat4 model = glm::mat4(1.0f);

        GLint projLoc = glGetUniformLocation(gridShaderProgram, "projection");
        GLint viewLoc = glGetUniformLocation(gridShaderProgram, "view");
        GLint modelLoc = glGetUniformLocation(gridShaderProgram, "model");

        glUniformMatrix4fv(projLoc, 1, GL_FALSE, glm::value_ptr(projection));
        glUniformMatrix4fv(viewLoc, 1, GL_FALSE, glm::value_ptr(view));
        glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(model));

        grid.draw();
    }

    void renderObjects() {
        glUseProgram(shaderProgram);

        glm::mat4 projection = glm::perspective(glm::radians(camera.getZoom()), 1200.0f / 800.0f, 0.1f, 100.0f);
        glm::mat4 view = camera.getViewMatrix();

        GLint projLoc = glGetUniformLocation(shaderProgram, "projection");
        GLint viewLoc = glGetUniformLocation(shaderProgram, "view");
        GLint modelLoc = glGetUniformLocation(shaderProgram, "model");
        GLint colorLoc = glGetUniformLocation(shaderProgram, "objectColor");

        glUniformMatrix4fv(projLoc, 1, GL_FALSE, glm::value_ptr(projection));
        glUniformMatrix4fv(viewLoc, 1, GL_FALSE, glm::value_ptr(view));

        for (auto& obj : objects) {
            glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(obj->modelMatrix));


            GLuint VBO, VAO, EBO;
            glGenVertexArrays(1, &VAO);
            glGenBuffers(1, &VBO);
            glGenBuffers(1, &EBO);

            glBindVertexArray(VAO);

            glBindBuffer(GL_ARRAY_BUFFER, VBO);
            glBufferData(GL_ARRAY_BUFFER, obj->vertices.size() * sizeof(float),
                &obj->vertices[0], GL_STATIC_DRAW);

            glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO);
            glBufferData(GL_ELEMENT_ARRAY_BUFFER, obj->indices.size() * sizeof(unsigned int),
                &obj->indices[0], GL_STATIC_DRAW);

            glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 3 * sizeof(float), (void*)0);
            glEnableVertexAttribArray(0);

            glDrawElements(GL_TRIANGLES, obj->indices.size(), GL_UNSIGNED_INT, 0);

            glDeleteVertexArrays(1, &VAO);
            glDeleteBuffers(1, &VBO);
            glDeleteBuffers(1, &EBO);
        }
    }

    // ===== USER INTERFACE =====
    void renderUI() {
        ImGui_ImplOpenGL3_NewFrame();
        ImGui_ImplGlfw_NewFrame();
        ImGui::NewFrame();

        // Main menu bar
        if (ImGui::BeginMainMenuBar()) {
            if (ImGui::BeginMenu("File")) {
                if (ImGui::MenuItem("New Scene")) {
                    for (auto obj : objects) {
                        delete obj;
                    }
                    objects.clear();
                    selectedObject = nullptr;
                    selectedObjectIndex = -1;
                }
                if (ImGui::MenuItem("Save as OBJ")) {
                    saveToOBJ("scene.obj");
                }
                if (ImGui::MenuItem("Load OBJ")) {
                    loadFromOBJ("scene.obj");
                }
                ImGui::EndMenu();
            }
            if (ImGui::BeginMenu("Edit")) {
                if (ImGui::MenuItem("Undo", "Ctrl+Z", false, commandHistory.canUndo())) {
                    commandHistory.undo();
                }
                if (ImGui::MenuItem("Redo", "Ctrl+Y", false, commandHistory.canRedo())) {
                    commandHistory.redo();
                }
                ImGui::EndMenu();
            }
            if (ImGui::BeginMenu("Tools")) {
                if (ImGui::MenuItem("Measure Distance")) {
                    measurementMode = true;
                    measurementStartSet = false;
                }
                ImGui::EndMenu();
            }
            if (ImGui::BeginMenu("Add")) {
                if (ImGui::MenuItem("Cube")) createCube();
                if (ImGui::MenuItem("Sphere")) createSphere();
                if (ImGui::MenuItem("Cylinder")) createCylinder();
                if (ImGui::MenuItem("Plane")) createPlane();
                ImGui::EndMenu();
            }
            ImGui::EndMainMenuBar();
        }

        // Object list panel
        ImGui::Begin("Scene Objects");

        if (ImGui::BeginListBox("##Objects", ImVec2(-FLT_MIN, 5 * ImGui::GetTextLineHeightWithSpacing()))) {
            for (int i = 0; i < objects.size(); i++) {
                const bool isSelected = (selectedObjectIndex == i);
                if (ImGui::Selectable(objects[i]->name.c_str(), isSelected)) {
                    selectedObjectIndex = i;
                    selectedObject = objects[i];
                    for (auto& obj : objects) {
                        obj->isSelected = false;
                    }
                    objects[i]->isSelected = true;
                }
            }
            ImGui::EndListBox();
        }

        if (ImGui::Button("Delete Selected") && selectedObject) {
            auto command = std::make_unique<DeleteObjectCommand>(objects, selectedObject);
            commandHistory.executeCommand(std::move(command));
            selectedObject = nullptr;
            selectedObjectIndex = -1;
        }

        ImGui::End();

        // Properties panel
        ImGui::Begin("Object Properties");

        if (selectedObject) {
            strcpy_s(nameBuffer, selectedObject->name.c_str());
            if (ImGui::InputText("Name", nameBuffer, sizeof(nameBuffer))) {
                selectedObject->name = std::string(nameBuffer);
            }

            float color[3] = { selectedObject->color.r, selectedObject->color.g, selectedObject->color.b };
            if (ImGui::ColorEdit3("Color", color)) {
                selectedObject->color = glm::vec3(color[0], color[1], color[2]);
            }

            ImGui::Separator();

            static float translation[3] = { 0.0f, 0.0f, 0.0f };
            static float rotation[3] = { 0.0f, 0.0f, 0.0f };
            static float scale[3] = { 1.0f, 1.0f, 1.0f };

            if (ImGui::DragFloat3("Translation", translation, 0.01f)) {
                glm::mat4 newTransform = glm::translate(glm::mat4(1.0f),
                    glm::vec3(translation[0], translation[1], translation[2]));

                auto command = std::make_unique<TransformCommand>(selectedObject,
                    selectedObject->modelMatrix, newTransform);
                commandHistory.executeCommand(std::move(command));
            }

            if (ImGui::DragFloat3("Rotation", rotation, 0.1f)) {
                glm::mat4 newTransform = glm::mat4(1.0f);
                newTransform = glm::rotate(newTransform, glm::radians(rotation[0]), glm::vec3(1.0f, 0.0f, 0.0f));
                newTransform = glm::rotate(newTransform, glm::radians(rotation[1]), glm::vec3(0.0f, 1.0f, 0.0f));
                newTransform = glm::rotate(newTransform, glm::radians(rotation[2]), glm::vec3(0.0f, 0.0f, 1.0f));

                auto command = std::make_unique<TransformCommand>(selectedObject,
                    selectedObject->modelMatrix, newTransform);
                commandHistory.executeCommand(std::move(command));
            }

            if (ImGui::DragFloat3("Scale", scale, 0.01f)) {
                glm::mat4 newTransform = glm::scale(glm::mat4(1.0f),
                    glm::vec3(scale[0], scale[1], scale[2]));

                auto command = std::make_unique<TransformCommand>(selectedObject,
                    selectedObject->modelMatrix, newTransform);
                commandHistory.executeCommand(std::move(command));
            }
        }
        else {
            ImGui::Text("No object selected");
        }

        ImGui::End();

        // Measurement display
        if (measurementStartSet || measurementMode) {
            ImGui::Begin("Measurement Tool");

            if (measurementStartSet) {
                ImGui::Text("Click to set end point");
                ImGui::Text("Start: (%.2f, %.2f, %.2f)",
                    measurementStart.x, measurementStart.y, measurementStart.z);
            }
            else if (measurementMode) {
                ImGui::Text("Click to set start point");
            }

            if (!measurementMode && measurementStartSet) {
                float distance = glm::length(measurementEnd - measurementStart);
                ImGui::Text("Distance: %.3f units", distance);
                ImGui::Text("Start: (%.2f, %.2f, %.2f)",
                    measurementStart.x, measurementStart.y, measurementStart.z);
                ImGui::Text("End: (%.2f, %.2f, %.2f)",
                    measurementEnd.x, measurementEnd.y, measurementEnd.z);

                if (ImGui::Button("New Measurement")) {
                    measurementMode = true;
                    measurementStartSet = false;
                }
            }

            ImGui::End();
        }

        ImGui::Render();
        ImGui_ImplOpenGL3_RenderDrawData(ImGui::GetDrawData());
    }

    // ===== SHADER CREATION =====
    GLuint createShaderProgram() {
        const char* vertexShaderSource = R"(
            #version 330 core
            layout (location = 0) in vec3 aPos;
            
            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;
            
            void main() {
                gl_Position = projection * view * model * vec4(aPos, 1.0);
            }
        )";

        const char* fragmentShaderSource = R"(
            #version 330 core
            out vec4 FragColor;
            
            uniform vec3 objectColor;
            
            void main() {
                FragColor = vec4(objectColor, 1.0);
            }
        )";

        GLuint vertexShader = glCreateShader(GL_VERTEX_SHADER);
        glShaderSource(vertexShader, 1, &vertexShaderSource, NULL);
        glCompileShader(vertexShader);

        GLuint fragmentShader = glCreateShader(GL_FRAGMENT_SHADER);
        glShaderSource(fragmentShader, 1, &fragmentShaderSource, NULL);
        glCompileShader(fragmentShader);

        GLuint program = glCreateProgram();
        glAttachShader(program, vertexShader);
        glAttachShader(program, fragmentShader);
        glLinkProgram(program);

        glDeleteShader(vertexShader);
        glDeleteShader(fragmentShader);

        return program;
    }

    GLuint createGridShaderProgram() {
        const char* gridVertexShaderSource = R"(
            #version 330 core
            layout (location = 0) in vec3 aPos;
            
            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;
            
            void main() {
                gl_Position = projection * view * model * vec4(aPos, 1.0);
            }
        )";

        const char* gridFragmentShaderSource = R"(
            #version 330 core
            out vec4 FragColor;
            
            void main() {
                FragColor = vec4(0.3, 0.3, 0.3, 1.0);
            }
        )";

        GLuint vertexShader = glCreateShader(GL_VERTEX_SHADER);
        glShaderSource(vertexShader, 1, &gridVertexShaderSource, NULL);
        glCompileShader(vertexShader);

        GLuint fragmentShader = glCreateShader(GL_FRAGMENT_SHADER);
        glShaderSource(fragmentShader, 1, &gridFragmentShaderSource, NULL);
        glCompileShader(fragmentShader);

        GLuint program = glCreateProgram();
        glAttachShader(program, vertexShader);
        glAttachShader(program, fragmentShader);
        glLinkProgram(program);

        glDeleteShader(vertexShader);
        glDeleteShader(fragmentShader);

        return program;
    }
};

// ===== MAIN FUNCTION =====
int main() {
    CADApplication app;

    if (!app.initialize()) {
        std::cerr << "Failed to initialize CAD application" << std::endl;
        return -1;
    }

    app.run();
    app.cleanup();

    return 0;
}